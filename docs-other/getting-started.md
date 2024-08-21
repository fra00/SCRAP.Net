# # Getting Started with SCRAP.Net

This guide will walk you through the process of setting up SCRAP.Net on your local machine. SCRAP.Net is a project that simulates robot communication with Arduino using HTTP protocol.

## Prerequisites

Before you begin, ensure you have the following installed:

- [Git](https://git-scm.com/)
- [Visual Studio 2022](https://visualstudio.microsoft.com/vs/) (or newer) with the .NET workload installed
- [.NET Core 7.0](https://dotnet.microsoft.com/download/dotnet/7.0)
- [eSpeak](https://espeak.sourceforge.net/)

## Installation Steps

### 1. Clone the Repository

1. Open a terminal or command prompt.

2. Navigate to the directory where you want to save the project.

3. Run the following command:
   
   ```
   git clone https://github.com/fra00/SCRAP.Net.git [your_path]
   ```
   
   Replace `[your_path]` with your desired directory path.

### 2. Open the Solution

1. Launch Visual Studio 2022 with administrator privileges.
2. Go to "File" -> "Open" -> "Project/Solution".
3. Navigate to the cloned repository and open the file:
   `[your_path]\SCRAP.Net\Robot\Robot.sln`

### 3. Configure MainRobot Project

#### Edit Configuration.cs

1. Open `Configuration.cs` in the MainRobot project.

2. Set the full path to eSpeak:
   
   ```csharp
   public static string PATH_ESPEAK = "C:\\Program Files (x86)\\eSpeak\\command_line\\";
   ```

3. Modify eSpeak TTS configuration arguments:
   
   ```csharp
   public static string ARGUMENTS_ESPEAK = "-v it -p 20 \"[@textToSpeach]\"";
   ```
   
   Note: `-v it` changes the language to Italian. Remove it for English.

4. Set the Arduino device IP (for emulator, leave as is):
   
   ```csharp
   public static string HTTP_URL_COMUNICATION = "http://192.168.1.238/?cmdData=";
   ```

5. Enable hardware simulation and remote Arduino simulation:
   
   ```csharp
   public static bool FAKE_HW = true;
   public static bool FAKE_REMOTE_ARDUINO = true;
   ```

#### Edit RobotConfiguration.cs

1. Open `RobotConfiguration.cs` in the MainRobot project.

2. Specify the path to load the map:
   
   ```csharp
   public static string MAP_FILE_NAME = "img//mappaMuri.png";
   ```

3. Set the maximum dimensions of the floor plan in cm:
   
   ```csharp
   public static int WIDHT_MAP = 1000;
   public static int HEIGHT_MAP = 1000;
   ```

4. Set the robot's dimensions in cm:
   
   ```csharp
   public static int HALF_WIDTH_ROBOT = 15;
   public static int HALF_HEIGHT_ROBOT = 15;
   ```

5. Leave the minimum movement size unchanged:
   
   ```csharp
   public static int MIN_STEP_FOR_FINDPATH = 10;
   ```

## Creating the Map

The map is a crucial element that you, as the user, need to create. It's used to support the lidar and sensors, and helps speed up pathfinding. Here's how to create and configure the map:

### Map Creation Guidelines

1. **Format**: The map should be a PNG image in black and white.
2. **Content**: 
   - Black areas represent walls or obstacles.
   - White areas represent the floor or navigable space.
3. **Dimensions**: 
   - The image size should be calculated based on the `WIDHT_MAP` and `HEIGHT_MAP` properties in `RobotConfiguration.cs`.
   - In the current configuration, one pixel corresponds to one centimeter.
4. **Creation Tool**: You can use any graphic editor to create this map.

### Steps to Create and Configure the Map

1. Create a black and white PNG image representing your environment. Ensure the dimensions match your `WIDHT_MAP` and `HEIGHT_MAP` settings.

2. Save the map file in your project directory, typically in an `img` folder.

3. Update the `MAP_FILE_NAME` in `RobotConfiguration.cs`:
   
   ```csharp
   public static string MAP_FILE_NAME = "img//yourMapFileName.png";
   ```

4. Ensure the `WIDHT_MAP` and `HEIGHT_MAP` properties in `RobotConfiguration.cs` match your map's dimensions in centimeters:
   
   ```csharp
   public static int WIDHT_MAP = 1000; // if your map is 1000 pixels wide
   public static int HEIGHT_MAP = 1000; // if your map is 1000 pixels tall
   ```

5. Adjust the robot's dimensions if necessary:
   
   ```csharp
   public static int HALF_WIDTH_ROBOT = 15;
   public static int HALF_HEIGHT_ROBOT = 15;
   ```

Remember, the accuracy of your map will directly impact the performance of the pathfinding algorithm and the overall simulation.

## Configure the Emulator

1. Right-click on the WindowFormRobotEmulator project and select "Properties".
2. Go to the "Resources" section.
   ![](.\getting-started\2024-08-20-14-29-23-image.png)
3. Update the map file resource (.png).
   ![](.\getting-started\2024-08-20-14-28-03-image.png)
4. Modify the path where SCRAP.NET is executed (should be the bin folder of the MainRobot project).
   
   ![](.\getting-started\2024-08-20-14-27-25-image.png)

## Run the Projects

1. Right-click on WindowFormRobotEmulator -> Debug -> Start New Instance
2. Right-click on MainRobot -> Debug -> Start New Instance

## Verification

If everything is set up correctly:

1. A shell should open, executing a series of commands.
2. The emulator should launch.

You should see a console window with various commands being executed, and the emulator interface should appear.

##![](.\getting-started\2024-08-20-14-46-18-image.png)

![](.\getting-started\2024-08-20-14-46-56-image.png)



#### Scaricare SCRAP.Net da GitHub

### Prerequisiti

- **Git:** Assicurati di avere Git installato sul tuo sistema. Puoi scaricarlo da [https://git-scm.com/](https://git-scm.com/).
- **Visual Studio:** Avrai bisogno di Visual Studio 2022 (o una versione più recente) con il workload .NET installato.

### Passaggi

1. **Scegliere una directory:**
   
   - Sul tuo computer, seleziona la cartella in cui desideri salvare il progetto.

2. **Aprire il terminale:**
   
   - Apri un terminale (o una finestra di comando) e naviga nella directory che hai scelto al punto 1.

3. **Clonare il repository:**
   
   - Copia e incolla il seguente comando nel terminale, sostituendo `[tuo_percorso]` con il percorso della directory che hai scelto:
     
     Bash
     
     ```
     git clone https://github.com/fra00/SCRAP.Net.git [tuo_percorso]
     ```
     
     Ad esempio:
     
     ```
     git clone https://github.com/fra00/SCRAP.Net.git C:\Progetti\SCRAP.Net
     ```

4. **Aprire la soluzione in Visual Studio:**
   
   - Una volta completato il download, apri Visual Studio.
   - Vai su "File" -> "Apri" -> "Progetto/Soluzione".
   - Naviga fino alla cartella dove hai clonato il progetto e seleziona il file solution (.sln).
     @yourpath\SCRAP.Net\Robot\Robot.sln

## Configurazione iniziale

### Prerequisiti

    installare eSpeak https://espeak.sourceforge.net/

Visual Studio 2022

.Net Core 7.0

### Configurazione progetto

In questo esempio configureremo un progetto che comunica con Arduino tramite protocollo Http e per verificarne il corretto funzionamento utilizzeremo l'emulatore

### 1. Aprire la soluzione con visual studio con permessi di amministratore

### 2. nel progetto MainRobot Aprire il file Configuration.cs

- Inserire il percorso completo di eSpeak

```
/// <summary>
/// to  speech by default is used eSpeak this is the folder when espeak is installed
/// </summary>
public static string PATH_ESPEAK = "C:\\Program Files (x86)\\eSpeak\\command_line\\";
```

- modificare gli argomenti per  la configurazione del TTS eSpeak
  -v it : cambia la lingua in italiano se si vuole in inglese(lingua di default) rimuoverlo

-p 20 : Adjusts the pitch in a range of 0 to 99. The default is 50.
se si vuole utilizzare in inglese lasciare solo il marker "[@textToSpeach]"

```
/// <summary>
/// parameter to configure eSpeak int this case is italian voice with pitch 20
/// </summary>
public static string ARGUMENTS_ESPEAK = "-v it -p 20 \"[@textToSpeach]\"";
```

- Inserire l'ip del dispositivo Arduino (in questo esempio verrà utilizzato solo l'emulatore quindi non serve sia presente o lasciare inalterato)

```
//if configured as Http use this as url to rest call
/// <summary>
/// http settings : url to server http arduino
/// </summary>
public static string HTTP_URL_COMUNICATION = "http://192.168.1.238/?cmdData=";
```

- Verificare che  vengano simulati gli hardware e che venga utilizzato arduino simulato in remoto

```
public static bool FAKE_HW = true;
public static bool FAKE_REMOTE_ARDUINO = true;
```

quando viene utilizzato l'emulatore in configurazione remota il comando viene scritto nel file "command.json" (nella cartella bin), che viene letto dall'emulatore, quando il file cambia l'emulatore rileva il comando inviato

- le altre configurazioni in questo caso d'uso possono essere tralasciate

### 3. nel progetto MainRobot Aprire il file RobotConfiguration.cs

- Specificare il path da dove caricare la mappa.
  La mappa viene usata a supporto del lidar e dei sensori, se presente serve a velocizzare il pathfinding che altrimenti si dovrebbe istruire rilevando gli ostacoli e aumentando il peso dei punti per l'algoritmo A*
  La mappa va fatta manualmente deve essere un png in bianco e nero,può essere fatto con qualsiasi editor grafico, dove il nero sono le pareti e bianco il pavimento , la dimensione dell'immagine deve essere calcoaltain base alla configurazione delle proprietà WIDHT_MAP:1000,HEIGHT_MAP:1000 , nella configurazione attuale un pixel corrisponde ad un centimetro.

```
/// <summary>
/// path of image used as map
/// </summary>
public static string MAP_FILE_NAME = "img//mappaMuri.png";
```

- Specificare la dimesnione massima nell'asse X della piantina in cm

```
/// <summary>
/// max width of map in cm
/// </summary>
public static int WIDHT_MAP = 1000;
```

- Specificare la dimensione massima nell'asse Y della piantina in cm

```
/// <summary>
/// max heeight of map in cm
/// </summary>
public static int HEIGHT_MAP = 1000;
```

- Specificare la dimesnione massima nell'asse X del robot in cm

```
/// <summary>
/// half width of robot in cm
/// </summary>
public static int HALF_WIDTH_ROBOT = 15;
```

- Specificare la dimensione massima nell'asse Y del robot in cm

```
/// <summary>
/// half height of robot in cm
/// </summary>
public static int HALF_HEIGHT_ROBOT = 15;
```

- Specifica la dimensione minima per lo spostamento minimo , lasciare invariato. é la dimensione dello spostamento minimo del robot 10 corrisponde a 10 cm e nella mappa corrisponde a 10 pixel. Corrisponde anche alla dimensione della griglia dell'emulatore. Lasciare inalterato

```
/// <summary>
/// min step use for pathfinding
/// </summary>
public static int MIN_STEP_FOR_FINDPATH = 10;
```

- le altre configurazioni in questo caso d'uso possono essere tralasciate

### 4. Configurazione emulatore

- Modificare le risorse del progetto WindowFormRobotEmulator quindi 
  Premere il tasto destro nel progetto WindowFormRobotEmulator e selezionare Proprietà
  Andare nella sezione risorse 



Aggiornare le risorse modificando il file della mappa ".png"



Modificare il path nel quale viene eseguito il SCRAP.NET , il path corrisponde alla cartella bin del progetto MainRobot





### 5. Eseguire il progetto WindowFormRobotEmulator (solo windows)

Click tasto destro su WindowFormRobotEmulator -> debug -> Avvia nuova istanza

### 6. Eseguire il progetto MainRobot

Click tasto destro su MainRobot-> debug -> Avvia nuova istanza





## Conclusione

Se tutto funziona in maniera corretta si dovrebbe aprire una shell che esegue una serie di comandi



e avviare l'emulatore
