using System.Net;
using System.Reflection;
using System.Text.Json;

namespace MainRobot.Http
{
    // Define a class for the http server
    public class HttpServer
    {
        // Declare an HttpListener field
        private HttpListener listener;

        // Declare a list of middleware fields
        private List<IMiddleware> middlewares;

        // Declare a service provider field
        private IServiceProvider provider;


        // Define a constructor that takes a prefix as a parameter
        public HttpServer(string prefix, IServiceProvider provider)
        {
            // Create an HttpListener and add the prefix
            listener = new HttpListener();
            listener.Prefixes.Add(prefix);

            // Create a list of middleware and add some default ones
            middlewares = new List<IMiddleware>();
            //middlewares.Add(new AuthenticationMiddleware());
            middlewares.Add(new ValidationMiddleware());
            middlewares.Add(new SerializationMiddleware());
            this.provider = provider;
        }

        // Define an async method to start the server
        public async Task StartAsync()
        {
            // Start the listener
            listener.Start();
            Console.WriteLine("Listening...");

            // Loop until the user presses Ctrl+C
            while (true)
            {
                // Get the context of the incoming request
                HttpListenerContext context = await listener.GetContextAsync();

                // Get the request and response objects
                HttpListenerRequest request = context.Request;
                HttpListenerResponse response = context.Response;

                // Create a dictionary to store the request data
                var data = new Dictionary<string, object>();

                // Invoke each middleware in the list and pass the context and data
                foreach (var middleware in middlewares)
                {
                    await middleware.InvokeAsync(context, data, provider);
                }

                // Close the response
                response.Close();
            }
        }
    }

    // Define an interface for the middleware
    interface IMiddleware
    {
        Task InvokeAsync(HttpListenerContext context, Dictionary<string, object> data, IServiceProvider provider);
    }

    //// Define a class for the authentication middleware
    //class AuthenticationMiddleware : IMiddleware
    //{
    //    public async Task InvokeAsync(HttpListenerContext context, Dictionary<string, object> data)
    //    {
    //        // Get the request and response objects
    //        HttpListenerRequest request = context.Request;
    //        HttpListenerResponse response = context.Response;

    //        // Check if the request has an authorization header
    //        if (request.Headers["Authorization"] != null)
    //        {
    //            // Get the authorization header value
    //            var auth = request.Headers["Authorization"];

    //            // Validate the authorization header value (dummy logic)
    //            if (auth == "Bearer 123456")
    //            {
    //                // Set the data dictionary with the user id (dummy value)
    //                data["UserId"] = 1;
    //                return; // Continue to the next middleware
    //            }
    //        }

    //        // Write an error response body
    //        var result = new { message = "Unauthorized" };
    //        var buffer = JsonSerializer.SerializeToUtf8Bytes(result);
    //        response.StatusCode = 401;
    //        response.ContentLength64 = buffer.Length;
    //        response.OutputStream.Write(buffer, 0, buffer.Length);

    //        throw new Exception("Unauthorized"); // Stop the middleware pipeline
    //    }
    //}

    // Define a class for the validation middleware
    class ValidationMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpListenerContext context, Dictionary<string, object> data, IServiceProvider provider)
        {
            // Get the request and response objects
            HttpListenerRequest request = context.Request;
            HttpListenerResponse response = context.Response;

            // Check if the request is a GET or DELETE
            if (request.HttpMethod == "GET" || request.HttpMethod == "DELETE")
            {
                // Read the querystring as a collection of keys and values
                var query = request.QueryString;
                if (query != null)
                {
                    foreach (var q in query.AllKeys)
                    {
                        data[q] = query[q];
                    }
                    return;
                }
                // Write an error response body
                var result = new { message = "Bad request" };
                var buffer = JsonSerializer.SerializeToUtf8Bytes(result);
                response.StatusCode = 400;
                response.ContentLength64 = buffer.Length;
                response.OutputStream.Write(buffer, 0, buffer.Length);

                throw new Exception("Bad request"); // Stop the middleware pipeline
            }

            // Check if the request is a POST or PUT
            if (request.HttpMethod == "POST" || request.HttpMethod == "PUT")
            {
                // Read the request body as a dynamic object
                var body = await JsonSerializer.DeserializeAsync<dynamic>(request.InputStream);

                // Validate the body object (dummy logic)
                if (body != null)
                {
                    // Set the data dictionary with the body object
                    data["body"] = body;
                    return; // Continue to the next middleware
                }

                // Write an error response body
                var result = new { message = "Bad request" };
                var buffer = JsonSerializer.SerializeToUtf8Bytes(result);
                response.StatusCode = 400;
                response.ContentLength64 = buffer.Length;
                response.OutputStream.Write(buffer, 0, buffer.Length);

                throw new Exception("Bad request"); // Stop the middleware pipeline
            }
        }
    }

    // Define a class for the serialization middleware
    public class SerializationMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpListenerContext context, Dictionary<string, object> data, IServiceProvider provider)
        {
            // Get the request and response objects
            HttpListenerRequest request = context.Request;
            HttpListenerResponse response = context.Response;

            // Get the controller name and action name from the request url
            var segments = request.Url.Segments;
            var controllerName = segments[1].TrimEnd('/');
            var actionName = segments[2].TrimEnd('/');

            // Get the controller type by name
            //var controllerType = Assembly.GetExecutingAssembly().GetType($"WebAPIServer.Controllers.{controllerName}Controller");
            var controllerType = Assembly.GetExecutingAssembly().GetTypes().Where(c => c.FullName.EndsWith(controllerName + "Controller")).FirstOrDefault();
            // Check if the controller type exists
            if (controllerType != null)
            {
                //// Create an instance of the controller type
                //var controller = Activator.CreateInstance(controllerType);

                // Get an instance of the controller type using dependency injection
                var controller = provider.GetService(controllerType);

                // Get the action method by name
                var actionMethod = controllerType.GetMethod(actionName);

                // Check if the action method exists
                if (actionMethod != null)
                {
                    // Get the attribute of the action method
                    var attribute = actionMethod.GetCustomAttribute<HttpMethodAttribute>();

                    // Check if the attribute exists and matches the request method
                    if (attribute != null && attribute.Method == request.HttpMethod)
                    {
                        // Invoke the action method and get the result object
                        var result = actionMethod.Invoke(controller, new object[] { data });

                        // Write the result object as a response body
                        var buffer = JsonSerializer.SerializeToUtf8Bytes(result);
                        response.ContentLength64 = buffer.Length;
                        response.OutputStream.Write(buffer, 0, buffer.Length);
                        return; // End the middleware pipeline
                    }
                }
            }

            // Write an error response body
            var error = new { message = "Not found" };
            var errorBuffer = JsonSerializer.SerializeToUtf8Bytes(error);
            response.StatusCode = 404;
            response.ContentLength64 = errorBuffer.Length;
            response.OutputStream.Write(errorBuffer, 0, errorBuffer.Length);

            throw new Exception("Not found"); // Stop the middleware pipeline
        }
    }

    // Define a class for the http method attribute
    [AttributeUsage(AttributeTargets.Method)]
    public class HttpMethodAttribute : Attribute
    {
        public string Method { get; }

        public HttpMethodAttribute(string method)
        {
            Method = method;
        }
    }



}
