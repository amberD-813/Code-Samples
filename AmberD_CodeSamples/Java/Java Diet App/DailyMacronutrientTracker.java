// Macro Nutrient Tracker
// I1 by Ryan Williams
// I2 Coded by Amber Davidson
// February 12th, 2020
// This program takes input for a daily goal for calories, protein, carbs and fat intake and lets a user keep track of their daily totals side-by-side with their goal.
// Developed and compiled using Netbeans 8.2
package daily.macronutrient.tracker;

import javafx.application.Application;
import javafx.application.Platform;
import javafx.event.ActionEvent;
import javafx.geometry.Insets;
import javafx.scene.Scene;
import javafx.scene.control.Button;
import javafx.stage.Stage;
import javafx.scene.control.Label;
import javafx.scene.layout.GridPane;
import javafx.scene.control.TextField;
import javafx.geometry.Pos;
import javafx.scene.control.Menu;
import javafx.scene.control.MenuBar;
import javafx.scene.control.MenuItem;
import javafx.scene.image.Image;
import javafx.scene.image.ImageView;
import javafx.scene.layout.HBox;
import javafx.scene.layout.VBox;
import javafx.scene.control.ProgressBar;
import javafx.scene.control.ProgressIndicator;

public class DailyMacronutrientTracker extends Application {

    Diet currentGoal = new Diet();      // Custom objects to store information about diets across classes
    Diet dailyDiet = new Diet();
    Diet clearAll = new Diet(); //Create Empty instance of class to clear form

    Label RootCaloriesLabel = new Label("Calories Intake: " + String.valueOf(dailyDiet.getCalories()) + "   -->");      // Labels on main screen that display daily totals.
    Label RootProteinLabel = new Label("Protein Intake: " + String.valueOf(dailyDiet.getProtein()) + "   --->");
    Label RootCarbsLabel = new Label("Carbs Intake: " + String.valueOf(dailyDiet.getCarbs()) + "   ----->");
    Label RootFatLabel = new Label("Fat Intake: " + String.valueOf(dailyDiet.getFat()) + "   ------->");

    Label RootCaloriesGoalLabel = new Label("Calories Goal: " + String.valueOf(currentGoal.getCalories()));     // Labels on main screen that display the current goal.
    Label RootProteinGoalLabel = new Label("Protein Goal: " + String.valueOf(currentGoal.getProtein()));
    Label RootCarbsGoalLabel = new Label("Carbs Goal: " + String.valueOf(currentGoal.getCarbs()));
    Label RootFatGoalLabel = new Label("Fat Goal: " + String.valueOf(currentGoal.getFat()));

    MenuBar menuBar = new MenuBar();//Create Menu Bar
    VBox vTop = new VBox(10);//Create Boxes for placement
    VBox vAte = new VBox(20);//VBox for displaying what was eaten
    VBox vGoal = new VBox(20);//VBox for displaying goals
    HBox hAll = new HBox(50);//HBox for goal and eatne vBoxes
    
    Label header = new Label(); //Holds name entered and header message 
    Label header2 = new Label("Daily Progress Tracker");
    Button eatBtn = new Button("What I Ate");        // Button to update daily totals.    
        
    @Override
    public void start(Stage primaryStage) {

        Button newGoalbtn = new Button();       // Button to create a new goal
        newGoalbtn.setText("My Goal");

        newGoalbtn.setOnAction((ActionEvent event) -> {
            NewGoal();
        } // Event handler that creates a new goal when the new Goal button is pushed;
        // See NewGoal() method below.
        );

        eatBtn.setOnAction((ActionEvent event) -> {
            IAte();
        } // Event handler that adjusts the totals of the total intake for the day;
        // See IAte() method below.
        );
        
        VBox root = new VBox(35);     // Main container
            
        hAll.setAlignment(Pos.CENTER);//Set Alignments
        hAll.setPadding(new Insets(10));
        vTop.setAlignment(Pos.TOP_LEFT);
        root.setAlignment(Pos.TOP_CENTER);

        HBox hMenu = getMenu(primaryStage);//Method to get menu
        vTop.getChildren().add(hMenu);//Add Menu to top box
        ImageView img = getImage();//Method to get Header Image
        vTop.getChildren().add(img);//Add image to top box in scene        
        vTop.setAlignment(Pos.CENTER);//Center Picture

        vGoal.getChildren().add(newGoalbtn);     // Add the buttons to the top of the container.
        vAte.getChildren().add(eatBtn); 
        eatBtn.setVisible(false);//Make button invisible until goal is filled out
        hAll.getChildren().addAll(vGoal,vAte);
        hAll.setAlignment(Pos.BOTTOM_CENTER);

        vAte.getChildren().add(RootCaloriesLabel);      // Display the labels to track totals vertically.
        vAte.getChildren().add(RootProteinLabel);
        vAte.getChildren().add(RootCarbsLabel);
        vAte.getChildren().add(RootFatLabel);

        vGoal.getChildren().add(RootCaloriesGoalLabel);      // Display the current goals vertically to the right of the totals.
        vGoal.getChildren().add(RootProteinGoalLabel);
        vGoal.getChildren().add(RootCarbsGoalLabel);
        vGoal.getChildren().add(RootFatGoalLabel);                   
        root.getChildren().addAll(vTop,hAll);
               
        Scene scene = new Scene(root, 700, 625);      // Create the scene and specify window size.

        primaryStage.setTitle("Daily Macronutrient Tracker");     // Create window.  
        scene.getStylesheets().add(getClass().getResource("macroNutrient_Style.css").toExternalForm());//Invoke CCS 
        primaryStage.setScene(scene);
        primaryStage.show();
    }

    public static void main(String[] args) {
        launch(args);
    }

    public void NewGoal() // This method creates a new goal and updates the goal on the main screen.
    {
        GridPane root = new GridPane();

        Scene newGoalScene = new Scene(root, 500, 500); // This window is a litte bigger since the controls take up more space; watch for clipping if you adjust these numbers!
        Stage newGoal = new Stage();

        Button setGoalbtn = new Button("Set Goal");     // Button to close screen and pass values.

        TextField CaloriesInput = new TextField();      // TextField input boxes for the new goal.
        TextField ProteinInput = new TextField();
        TextField CarbsInput = new TextField();
        TextField FatInput = new TextField();

        Label CaloriesLabel = new Label("Enter Daily Calories: ");      // Label fields that indicated which TextField is which.
        Label ProteinLabel = new Label("Enter Daily Protein: ");
        Label CarbsLabel = new Label("Enter Daily Carbs: ");
        Label FatLabel = new Label("Enter Daily Fat: ");

        root.setVgap(50);       // Formatting for controls.
        root.setPadding(new Insets(25, 25, 25, 15));

        root.add(CaloriesLabel, 0, 0);      // Add all controls in an orderly fashion.
        root.add(CaloriesInput, 1, 0);

        root.add(ProteinLabel, 0, 1);
        root.add(ProteinInput, 1, 1);

        root.add(CarbsLabel, 0, 2);
        root.add(CarbsInput, 1, 2);

        root.add(FatLabel, 0, 3);
        root.add(FatInput, 1, 3);

        root.add(setGoalbtn, 1, 4);

        newGoal.setTitle("New Goal");       // Create window.
        newGoalScene.getStylesheets().add(getClass().getResource("macroNutrient_Style.css").toExternalForm());//Invoke CCS
        newGoal.setScene(newGoalScene);
        newGoal.show();

        setGoalbtn.setOnAction((ActionEvent event) -> {
            // Then updates the labels on the main screen and closes the window.
            currentGoal.setCalories(Double.parseDouble(CaloriesInput.getText()));
            currentGoal.setProtein(Double.parseDouble(ProteinInput.getText()));
            currentGoal.setCarbs(Double.parseDouble(CarbsInput.getText()));
            currentGoal.setFat(Double.parseDouble(FatInput.getText()));
            UpdateGoal();
            newGoal.close();
            eatBtn.setVisible(true);
        } // Event handler for set Goal button
        // Sets currentGoal object values to user specified numbers.
        );

    }

    public void IAte() // This method adds to the current totals on the main screen.
    {
        GridPane root = new GridPane();

        Scene IAteScene = new Scene(root, 700, 500);        // This window is a litte bigger since the controls take up more space; watch for clipping if you adjust these numbers!
        Stage IAte = new Stage();

        Button Submitbtn = new Button("Submit");  // Button to close windows and submit values.

        TextField CaloriesInput = new TextField();      // TextField inputs for current Totals.
        TextField ProteinInput = new TextField();
        TextField CarbsInput = new TextField();
        TextField FatInput = new TextField();

        Label CaloriesLabel = new Label("Enter how many calories you ate:");        // Labels to indicate which TextField is which.
        Label ProteinLabel = new Label("Enter how much protein your food had: ");
        Label CarbsLabel = new Label("Enter how many carbs your food had: ");
        Label FatLabel = new Label("Enter how much fat your food had: ");

        root.setVgap(50);       // Formatting for controls.
        root.setPadding(new Insets(25, 25, 25, 15));

        root.add(CaloriesLabel, 0, 0);      // Add each control in an orderly fashion.
        root.add(CaloriesInput, 1, 0);

        root.add(ProteinLabel, 0, 1);
        root.add(ProteinInput, 1, 1);

        root.add(CarbsLabel, 0, 2);
        root.add(CarbsInput, 1, 2);

        root.add(FatLabel, 0, 3);
        root.add(FatInput, 1, 3);

        root.add(Submitbtn, 1, 4);

        IAte.setTitle("Food Logging");          // Create window.
        IAteScene.getStylesheets().add(getClass().getResource("macroNutrient_Style.css").toExternalForm());//Invoke CCS
        IAte.setScene(IAteScene);
        IAte.show();

        Submitbtn.setOnAction((ActionEvent event) -> {
            dailyDiet.addCalories(Double.parseDouble(CaloriesInput.getText()));
            dailyDiet.addProtein(Double.parseDouble(ProteinInput.getText()));
            dailyDiet.addCarbs(Double.parseDouble(CarbsInput.getText()));
            dailyDiet.addFat(Double.parseDouble(FatInput.getText()));
            UpdateCounts();
            IAte.close();
        } // Event handler for submit button that sends values to dailyDiet object
        // and also updates the totals on the main screen then closes the window.
        );
    }

    public void UpdateCounts() // Method to update the Labels for daily totals on the main screen
    {
        RootCaloriesLabel.setText("Calories Intake: " + String.valueOf(dailyDiet.getCalories()));
        RootProteinLabel.setText("Protein Intake: " + String.valueOf(dailyDiet.getProtein()));
        RootCarbsLabel.setText("Carbs Intake: " + String.valueOf(dailyDiet.getCarbs()));
        RootFatLabel.setText("Fat Intake: " + String.valueOf(dailyDiet.getFat()));
        getProgress();
    }

    public void UpdateGoal() // Method to update the goal.
    {
        RootCaloriesGoalLabel.setText("Calories Intake: " + String.valueOf(currentGoal.getCalories()));
        RootProteinGoalLabel.setText("Protein Intake: " + String.valueOf(currentGoal.getProtein()));
        RootCarbsGoalLabel.setText("Carbs Intake: " + String.valueOf(currentGoal.getCarbs()));
        RootFatGoalLabel.setText("Fat Intake: " + String.valueOf(currentGoal.getFat()));
        
    }
    public void getProgress()
    {
        //Progress Bars
        ProgressBar pbCal = new ProgressBar(dailyDiet.getCalories()/currentGoal.getCalories());//Progress bar for calories
        ProgressBar pbPro = new ProgressBar(dailyDiet.getProtein()/currentGoal.getProtein());//Progress bar for protien
        ProgressBar pbCar = new ProgressBar(dailyDiet.getCarbs()/currentGoal.getCarbs());//Progress bar for Carbs
        ProgressBar pbFat = new ProgressBar(dailyDiet.getFat()/currentGoal.getFat());//Progress bar for Fat
        
        //Progress Percentage Indicators
        ProgressIndicator piCal = new ProgressIndicator(pbCal.getProgress());
        ProgressIndicator piPro = new ProgressIndicator(pbPro.getProgress());
        ProgressIndicator piCar = new ProgressIndicator(pbCar.getProgress());
        ProgressIndicator piFat = new ProgressIndicator(pbFat.getProgress());
        
        //Labels
        Label lblCalProgress = new Label("Calorie Progress: ");//Label for Calorie bar
        Label lblProProgress = new Label("Protien Progress: ");//Label for Calorie bar
        Label lblCarProgress = new Label("Carb Progress:     ");//Label for Calorie bar
        Label lblFatProgress = new Label("Fat Progress:       ");//Label for Calorie bar      
        header2.setId("label-header2");
        
        //HBoxes
        HBox hCalProg = new HBox(20,lblCalProgress,pbCal,piCal);//Calorie Box
        hCalProg.setAlignment(Pos.CENTER);
        HBox hProProg = new HBox(20,lblProProgress,pbPro,piPro);//Protien Box
        hProProg.setAlignment(Pos.CENTER);
        HBox hCarProg = new HBox(20,lblCarProgress,pbCar,piCar);//Carb Box
        hCarProg.setAlignment(Pos.CENTER);
        HBox hFatProg = new HBox(20,lblFatProgress,pbFat,piFat);//Fat Box
        hFatProg.setAlignment(Pos.CENTER);
        VBox vProgress = new VBox(25);//VBox for all Progress Bars
        
        vProgress.setAlignment(Pos.CENTER);//Set Box Alignemnt 
        vProgress.getChildren().addAll(header2,hCalProg,hProProg,hCarProg,hFatProg);//Add all bars and labels to box
      
        Scene ProgressScene = new Scene(vProgress, 600, 500);        // This window is a litte bigger since the controls take up more space; watch for clipping if you adjust these numbers!
        Stage Progress = new Stage();
        Progress.setTitle("Intake Progress");          // Create window.
        ProgressScene.getStylesheets().add(getClass().getResource("macroNutrient_Style.css").toExternalForm());//Invoke CCS
        Progress.setScene(ProgressScene);
        Progress.show();
        
    }
    public void clearAll()//Method to clear all from top menu
    {
        String zero = "0";
        
        dailyDiet.setCalories(Double.parseDouble(zero));
        dailyDiet.setProtein(Double.parseDouble(zero));
        dailyDiet.setCarbs(Double.parseDouble(zero));
        dailyDiet.setFat(Double.parseDouble(zero));
        UpdateCounts();
        currentGoal.setCalories(Double.parseDouble(zero));
        currentGoal.setProtein(Double.parseDouble(zero));
        currentGoal.setCarbs(Double.parseDouble(zero));
        currentGoal.setFat(Double.parseDouble(zero));
        UpdateGoal();           
    }
    public void getName()//Method to get user name on manu selection 
    {
        TextField txtFName = new TextField();//Field for First Name
        TextField txtLName = new TextField();//Field for Last Name
        Label lblFName = new Label("First Name:");//LAbel for First Name
        Label lblLName = new Label("Last Name:");//Label for Last Name
        Button btnEnter = new Button("Enter");
        header.setId("label-header");
        vTop.setAlignment(Pos.CENTER);
        HBox hNames = new HBox(20,lblFName,txtFName,lblLName,txtLName);//Box for Text Fields      
        vTop.getChildren().addAll(hNames,btnEnter);//Add to Box in scene
        
        
        btnEnter.setOnAction((ActionEvent event) -> { //Enter Name Event Handler           
        
        if (txtFName.getText().trim().length()>0&&txtLName.getText().trim().length()>0)//Ensure names entered
        {           
            header.setText(txtFName.getText().trim()+" "+txtLName.getText().trim()+"'s Diet Tracker");//Show name in header           
            vTop.getChildren().add(header);//Add to Box in scene
            vTop.getChildren().remove(hNames);//Remove text Felds
            vTop.getChildren().remove(btnEnter);//Remove text Felds
        }
        });
    }
    public void getInfo()//Method to show box with more info
    {
        Label lblMoreInfo = new Label("MacroNutrient Tracker Iteration2....\n\n--Amber D.");
        Button btnReturn = new Button("Return");
        ImageView img2 = getImage2();//Method to get Header Image      
        VBox hMessage = new VBox(20,lblMoreInfo,img2,btnReturn);//Add all to box
        hMessage.setAlignment(Pos.CENTER);//Set position
           
       
        Scene informationScene = new Scene(hMessage, 500, 400);   // Set Scene
        Stage information = new Stage();
        information.setTitle("More Info");   // Create window.
        informationScene.getStylesheets().add(getClass().getResource("macroNutrient_Style.css").toExternalForm());//Invoke CCS 
        information.setScene(informationScene);
        information.show();
        
          btnReturn.setOnAction((ActionEvent event) -> { //Return Button Event Handler 
          information.close();
        });
        
    }
    
    public ImageView getImage()//Get Image and return it to scene
    {     
        Image imgMusic = new Image("/Macronutrients.jfif");//Create image object
        ImageView iView = new ImageView(imgMusic);//Create ImageView object             
        iView.setFitWidth(700);
        iView.setFitHeight(135);      
        
        return iView;
    }
    public ImageView getImage2()//Get Image and return it to scene
    {     
        Image imgMusic = new Image("/smiley.jfif");//Create image object
        ImageView iView = new ImageView(imgMusic);//Create ImageView object             
        iView.setFitWidth(75);
        iView.setFitHeight(75);      
        
        return iView;
    }
    public HBox getMenu(Stage primaryStage) //Method to create Menu bar and hold action events
    {
        Menu mnuFile = new Menu("File");//Create MenuInstance
        mnuFile.setId("label-file");
        //Cretae Menu Items
        MenuItem mnuName = new MenuItem("Enter Name");
        MenuItem mnuInfo = new MenuItem("More Info");
        MenuItem mnuClear = new MenuItem("Clear All");
        MenuItem mnuExit = new MenuItem("Exit");

        //Add Menu Items        
        mnuFile.getItems().add(mnuName);
        mnuName.setId("label-name");
        mnuFile.getItems().add(mnuInfo);
        mnuFile.getItems().add(mnuClear);
        mnuFile.getItems().add(mnuExit);
        menuBar.getMenus().add(mnuFile);

        mnuExit.setOnAction((ActionEvent event) -> { //Exit Menu Event Handler 
            primaryStage.close();
            Platform.exit();
        });
        mnuClear.setOnAction((ActionEvent event) -> { //Clear All Event Handler 
            clearAll();
        });
        mnuName.setOnAction((ActionEvent event) -> { //Clear All Event Handler 
            getName();
        });
         mnuInfo.setOnAction((ActionEvent event) -> { //Clear All Event Handler 
            getInfo();
        });
              
        HBox menu = new HBox(menuBar);//Add Menu to VBox
        

        return menu;//Return HBox with menu to scene

    }

    public class Diet // Class to store data about the diet across the program.
    {

        double calories, protein, carbs, fat;

        Diet() // Default constructor used at compile time
        {
            this.calories = 0;
            this.protein = 0;
            this.carbs = 0;
            this.fat = 0;
        }

        Diet(double calories, double protein, double carbs, double fat) // This constructor isn't used anymore, but is kept in case it is needed later.
        {
            this.calories = calories;
            this.protein = protein;
            this.carbs = carbs;
            this.fat = fat;
        }

        public double getCalories() {
            return calories;
        }        // Accessor methods for all fields.

        public double getProtein() {
            return protein;
        }

        public double getCarbs() {
            return carbs;
        }

        public double getFat() {
            return fat;
        }

        public void setCalories(double calories) {
            this.calories = calories;
        }          // Mutator methods for all fields.

        public void setProtein(double protein) {
            this.protein = protein;
        }

        public void setCarbs(double carbs) {
            this.carbs = carbs;
        }

        public void setFat(double fat) {
            this.fat = fat;
        }

        public void addCalories(double calsToAdd) {
            this.calories += calsToAdd;
        }       // Methods to add a value to each field.

        public void addProtein(double proToAdd) {
            this.protein += proToAdd;
        }

        public void addCarbs(double carbsToAdd) {
            this.carbs += carbsToAdd;
        }

        public void addFat(double fatToAdd) {
            this.fat += fatToAdd;
        }
    }

}
