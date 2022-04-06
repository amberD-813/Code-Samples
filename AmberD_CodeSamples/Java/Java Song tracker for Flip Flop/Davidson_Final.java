/*                                
Amber Davidson
Final 5/6/2020
This program is the final
Developed and compiled using Netbeans 8.2
 */
package davidson_final;

import java.io.File;
import java.net.URL;
import java.sql.Connection;
import java.sql.DriverManager;
import java.sql.ResultSet;
import java.sql.ResultSetMetaData;
import java.sql.SQLException;
import java.sql.Statement;
import java.util.logging.Level;
import java.util.logging.Logger;
import javafx.scene.image.Image;
import javafx.scene.image.ImageView;
import javafx.application.Application;
import javafx.beans.property.SimpleStringProperty;
import javafx.beans.value.ObservableValue;
import javafx.collections.FXCollections;
import javafx.collections.ObservableList;
import javafx.event.ActionEvent;
import javafx.geometry.Insets;
import javafx.geometry.Pos;
import javafx.scene.Scene;
import javafx.scene.control.Alert;
import javafx.scene.control.Alert.AlertType;
import javafx.scene.control.Button;
import javafx.scene.control.CheckBox;
import javafx.scene.control.Label;
import javafx.scene.control.MenuBar;
import javafx.scene.control.Menu;
import javafx.scene.control.MenuItem;
import javafx.scene.control.RadioButton;
import javafx.scene.control.TableColumn;
import javafx.scene.control.TableView;
import javafx.scene.control.TextArea;
import javafx.scene.control.TextField;
import javafx.scene.control.ToggleGroup;
import javafx.scene.control.TreeItem;
import javafx.scene.control.TreeView;
import javafx.scene.layout.HBox;
import javafx.scene.layout.VBox;
import javafx.stage.FileChooser;
import javafx.stage.Stage;
import javafx.util.Callback;

public class Davidson_Final extends Application {

//Global Variables  
//Boxes    
HBox hbText = new HBox(20); 
HBox hbRadio = new HBox(60);
HBox hbComments = new HBox(20);
HBox hMenu = new HBox();
VBox main = new VBox(50);

//First Column of HBoxes
HBox h1 = new HBox(20);//HBox to hold Check Box & Text Box
HBox h3 = new HBox(20);//HBox to hold Check Box & Text Box
HBox h5 = new HBox(20);//HBox to hold Check Box & Text Box
//Start Second Set of Boxes
HBox h2 = new HBox(20);///HBox to hold Check Box & Text Box
HBox h4 = new HBox(20);//HBox to hold Check Box & Text Box
HBox h6 = new HBox(20);//HBox to hold Check Box & Text Box 
        
//Buttons 
Button btnSave = new Button("Save Song");
Button btnLyrics = new Button("Save Lyrics");
RadioButton radioOG = new RadioButton ("Original");  
RadioButton radioCover = new RadioButton ("Cover");
//Text Fields
TextField textName = new TextField();
TextArea txtComments = new TextArea();
TextField txtLead = new TextField();
TextField txtRhythm = new TextField();
TextField txtBass = new TextField();
TextField txtDrums = new TextField();
TextField txtBanjo = new TextField();
TextField txtVocals = new TextField();
//Check Boxes
CheckBox guitarLead = new CheckBox("Guitar 1");
CheckBox drums = new CheckBox("Drums");
CheckBox bass = new CheckBox("Bass");
CheckBox guitarRhythm = new CheckBox("Guitar 2");
CheckBox vocals = new CheckBox("Vocals");
CheckBox banjo = new CheckBox("Banjo");
//SQL
String sqlString = "";//Variable for SQL statement
 //File
private File fileLyrics;
   
    @Override
    public void start(Stage primaryStage) {
              
        hMenu = getMenus(primaryStage);//Method to get menu
        getBoxes();//Call Method to Populate Home Screen 
            
        btnSave.setOnAction((ActionEvent event) -> {try {
            //Save Song
            getInfo();//Method to collect all info entered
            } catch (SQLException ex) {
                Logger.getLogger(Davidson_Final.class.getName()).log(Level.SEVERE, null, ex);
            }     
       }); 
        btnLyrics.setOnAction((ActionEvent event) -> {//Save Lyric File
            
        saveTxt(primaryStage);//Method to collect all info entered
                 
       });
              
        //Set and Show Scene
        Scene scene = new Scene(main,750,875);
        scene.getStylesheets().add(getClass().getResource("Final_Style_Sheet.css").toExternalForm());//Invoke CCS   
        primaryStage.setTitle("Song Catalog");//Set Title
        primaryStage.setScene(scene);//Set Scene
        primaryStage.show();
    }
    
      public VBox getBoxes()//Get Boxes and buttons to create Main window
    {   
        //HBox homeMenu = getMenus();//HBox for Menu      
        ImageView img = getImage();//Method to get Header Image
        
        Label header = new Label("Flip Flop Repair Shop");//Set Header Text        
        Label lblName = new Label("Song Name:");//Text Box Label    
        Label lblComments = new Label("Comments:");
        txtComments.setWrapText(true);//Multiline
        txtComments.setMaxWidth(200);
        txtComments.setMaxHeight(50);
        header.setId("label-header");//Set CCS Label for header
        VBox spacer = new VBox(5);
        //main.setPadding(new Insets(20));
        
        VBox top = new VBox(25,header,img);//Box for Header
        top.setAlignment(Pos.TOP_CENTER);
        ToggleGroup toggleData = new ToggleGroup();//Declare Toggle Group for Data
        radioOG.setToggleGroup(toggleData);//Assign OG RadioButton
        radioCover.setToggleGroup(toggleData);//Assign Cover RadioButton
        hbRadio.getChildren().addAll(radioOG, radioCover);//Add Radio Buttons to box
        hbRadio.setAlignment(Pos.CENTER);//Set Alignment 
        hbRadio.setSpacing(10);//Set Spacing
        hbText.getChildren().addAll(lblName, textName);//Add Label and text to box
        hbText.setAlignment(Pos.BOTTOM_CENTER);//Set Alignment 
        hbText.setSpacing(10);//Spacing
        hbComments.getChildren().addAll(lblComments,txtComments,spacer);//HBox for Comments
        hbComments.setAlignment(Pos.BOTTOM_CENTER);//Set Alignment
        HBox hButtons = new HBox(50,btnLyrics,btnSave);
        hButtons.setAlignment(Pos.CENTER);
        
        HBox members = getMembers();//Method to get Member and Instrument Check Boxes      
       
        main.getChildren().addAll(hMenu,top,hbText,members,hbRadio,hbComments,hButtons);//Set Main Box
        main.setAlignment(Pos.TOP_CENTER);//Set Alignment        
        
        return main;
    }

    public HBox getMembers()//Get Instruments and Members
    {        
        h1.getChildren().add(guitarLead);        
        h3.getChildren().add(drums);      
        h5.getChildren().add(bass);
        h2.getChildren().add(guitarRhythm);
        h4.getChildren().add(vocals);
        h6.getChildren().add(banjo);

        //Populate First Column Text Boxes
        guitarLead.setOnAction((ActionEvent event) -> { //CheckBox Event Handler      
        if (guitarLead.isSelected())//Check Guitar Selected
        {
            h1.getChildren().addAll(txtLead);//Enter Lead Guitar Name, Show Text Box
        }
        else
        {
            h1.getChildren().remove(txtLead);//Hide TextBox
            txtLead.clear();
        }
        });
        drums.setOnAction((ActionEvent event) -> { //CheckBox Event Handler 
        if (drums.isSelected())//Check drums selected
        {
            h3.getChildren().addAll(txtDrums);//Enter Drum Name, Show Text Box                  
        }
        else
        {
            h3.getChildren().remove(txtDrums);//Remove Text Box     
            txtDrums.clear();
        }
        });
        bass.setOnAction((ActionEvent event) -> { //CheckBox Event Handler
        if (bass.isSelected())//Check Bass Selected
        {
            h5.getChildren().addAll(txtBass);//Enter Bass Name, Show Text Box     
        } 
        else
        {
            h5.getChildren().remove(txtBass);//Hide Text Box   
            txtBass.clear();
        }
        });
        
        guitarRhythm.setOnAction((ActionEvent event) -> { //CheckBox Event Handler
        if (guitarRhythm.isSelected())//Check Rhythm SelectedVBox v4 = new VBox(30);//VBox to hold Member Name Text Boxes
        {
            h2.getChildren().addAll(txtRhythm);//Enter Guitar 2 Name, Show Text Box
        }
        else
        {
            h2.getChildren().remove(txtRhythm);//Hide TextBox
            txtRhythm.clear();
        }       
        });
        vocals.setOnAction((ActionEvent event) -> { //CheckBox Event Handler 
        if (vocals.isSelected())//Check vocals selected
        {
            h4.getChildren().addAll(txtVocals);//Enter Vocals Name, Show Text Box         
        }
        else
        {
            h4.getChildren().remove(txtVocals);//Hide TextBox
            txtVocals.clear();
        } 
        });
        banjo.setOnAction((ActionEvent event) -> { //CheckBox Event Handler 
        if (banjo.isSelected())//Check Banjo Selected
        {
            h6.getChildren().addAll(txtBanjo);//Enter Banjo Name, Show Text Box          
        } 
        else
        {
            h6.getChildren().remove(txtBanjo);//Hide TextBox
            txtBanjo.clear();
        }         
        });
        VBox Column1 = new VBox(20,h1,h3,h5);//VBox containing HBoxes
        VBox Column2 = new VBox(20,h2,h4,h6);//VBox containing HBoxes
        HBox allMembers = new HBox(40,Column1, Column2);//Set All VBox
        allMembers.setAlignment(Pos.CENTER);
        
        return allMembers;//Return Total Members Box
    }
    
    public HBox getMenus(Stage primaryStage)//Method to get Menu 
    {
        Menu mnuFile = new Menu("File");//Create MenuInstance
        //Cretae Menu Items
        MenuItem mnuAbout = new MenuItem("About");
        MenuItem mnuList = new MenuItem("Song List");
        MenuItem mnuExit = new MenuItem("Exit");
        
        MenuBar menuBar = new MenuBar();//Create Menu Bar
        //Add Menu Items               
        mnuFile.getItems().add(mnuList);
        mnuFile.getItems().add(mnuAbout);
        mnuFile.getItems().add(mnuExit);
        menuBar.getMenus().add(mnuFile);  
        
        HBox menu = new HBox(menuBar);//Add Menu to VBox     
        
        mnuExit.setOnAction((ActionEvent event) -> { //CheckBox Event Handler 
                primaryStage.close();        
        });
        mnuAbout.setOnAction((ActionEvent event) -> { //CheckBox Event Handler 
                getAbout();        
        }); 
        mnuList.setOnAction((ActionEvent event) -> { //CheckBox Event Handler 
                showSongList();        
        });
                
        return menu;
    }
    
    public void showSongList(){//Method to Show song list and button Event to show song details
        
        VBox vList = new VBox(20);//Box  
        Button btnDetails = new Button("Song Details");
        vList.setAlignment(Pos.CENTER);
        TableView tableView = new TableView();//Table
        ObservableList<ObservableList>data = FXCollections.observableArrayList();
        
        //Get rid of previous data           
        data.removeAll(data);//Clear table
        tableView.getItems().clear();
        tableView.getColumns().clear();
        
     try
      {            
          sqlString = "SELECT * FROM TBL_SONG";//Set SQL string
          ResultSet rsList = getDatabase(2);//Get Song List from database
          
           //Create Columns
           if (rsList != null){
            for (int i = 0; i < rsList.getMetaData().getColumnCount(); i++) {
               final int j = i;                
                TableColumn col = new TableColumn(rsList.getMetaData().getColumnName(i+1)); //Get Column names      
                  col.setCellValueFactory(new Callback<TableColumn.CellDataFeatures<ObservableList, String>, ObservableValue<String>>() {
                    public ObservableValue<String> call(TableColumn.CellDataFeatures<ObservableList, String> param) {
                        return new SimpleStringProperty(param.getValue().get(j).toString());
                    }
                });
                tableView.setColumnResizePolicy( TableView.CONSTRAINED_RESIZE_POLICY );
                col.setMinWidth(150);//Set Column Width
                tableView.getColumns().addAll(col);//Add Columns            
            }
           }              
        while (rsList.next()) {//While Results
                
                ObservableList<String> row = FXCollections.observableArrayList();
                for (int i = 1; i <= rsList.getMetaData().getColumnCount(); i++) {//Iterate Row
                    
                    row.add(rsList.getString(i));//Add row
                }                   
                    data.add(row);//Add row to list
            }
          
            tableView.setItems(data);//Add list to tableView                        
         
          }catch(Exception e){
              e.printStackTrace();
              System.out.println("Error Getting Data");             
          }        
       
        vList.getChildren().addAll(tableView,btnDetails);//Add table to scene
        
        
         btnDetails.setOnAction((ActionEvent event) -> { //Button Event Handler 
             
             String selection =  tableView.getSelectionModel().selectedItemProperty().get().toString();//Get Selected Song
             int left =  selection.indexOf("[");//Parse Song Name between left
             int right =  selection.indexOf(",");//Parse Song Name between right             
             String songSelect = selection.substring(left +1, right);//Parse Song Name
             
            try {
                handle(songSelect);//Event Do Stuff Method
            } catch (SQLException ex) {
                Logger.getLogger(Davidson_Final.class.getName()).log(Level.SEVERE, null, ex);
            }                   
        });
        

        Scene listScene = new Scene(vList, 500, 500); // This window is a litte bigger since the controls take up more space; watch for clipping if you adjust these numbers!
        Stage newList = new Stage();
        
        newList.setTitle("Song List");       // Create window.
        listScene.getStylesheets().add(getClass().getResource("Final_Style_Sheet_List.css").toExternalForm());//Invoke CCS
        newList.setScene(listScene);
        newList.show();
    }
    
    public void getInfo() throws SQLException//Method to Check if Song Already Exists and Retrieve Info Entered
    {   
        String songName = "";
        String columnValue = "";
        ResultSet rsSong = null;
        songName = textName.getText().trim().toUpperCase();//Get Song Name Uppercase because primary key        
                
        sqlString = "SELECT SONG_NAME FROM TBL_SONG WHERE SONG_NAME = '" + songName + "'"; //SQL to look for song
        rsSong = getDatabase(2);//database method with argument to retrieve data
        ResultSetMetaData rsmd = rsSong.getMetaData();
        int columnsNumber = rsmd.getColumnCount();
        System.out.println(columnsNumber);
        
        while (rsSong.next() && songName != "") {
                       
          for (int i = 1; i <= columnsNumber; i++) {
             
              columnValue = rsSong.getString(i);             
              }             
        }   
                 
        if (validate() == true && columnValue == ""){//Method to Check for Required Info
                     
                    sqlString = "";
                    collectData();
        } 
        else if (songName.length() != 0){//Alert User of song duplicate
            
        Alert alert = new Alert(AlertType.INFORMATION);
        alert.setTitle("Error");
        alert.setHeaderText("Song Already Exists");
        alert.setContentText("Please Enter a Different Song....");
        alert.show();      
    }            
  }
    
    private void collectData() throws SQLException//Method to get data from text boxes
    {
        String[][] arrayMusic = new String[6][2];    
        String songName = null;
        String comments = null;
        
        songName = textName.getText().trim().toUpperCase();//Get Song Name Uppercase because primary key 
        comments = txtComments.getText().trim();//Get Comments
        String songType = "";//Song Type Variable
        
        //Song Type Analysis
        if (radioOG.isSelected()){
            songType = "Original";
        }
        if (radioCover.isSelected()){
            songType = "Cover";
        }

        if (guitarLead.isSelected()){
              arrayMusic[0][0] = "Guitar 1";
              arrayMusic[0][1] = txtLead.getText().trim();            
        }
        if (drums.isSelected()){
              arrayMusic[1][0] = "Drums";
              arrayMusic[1][1] = txtDrums.getText().trim();            
        }       
        if (bass.isSelected()){
              arrayMusic[2][0] = "Bass";
              arrayMusic[2][1] = txtBass.getText().trim();            
        }
        if (guitarRhythm.isSelected()){
              arrayMusic[3][0] = "Guitar 2";
              arrayMusic[3][1] = txtRhythm.getText().trim();            
        }
        if (vocals.isSelected()){
              arrayMusic[4][0] = "Vocals";
              arrayMusic[4][1] = txtVocals.getText().trim();            
        }
        if (banjo.isSelected()){
              arrayMusic[5][0] = "Banjo";
              arrayMusic[5][1] = txtBanjo.getText().trim();            
        }
        
        try{
            
      if (songName != null && songType.length()>=5 && arrayMusic != null){
          
                  sqlString = "INSERT INTO TBL_SONG (SONG_NAME, SONG_TYPE, COMMENTS)" //Save Song to Song Table
                   + "VALUES ('" + songName + "','" + songType + "','" + comments + "')";          
                   
                   getDatabase(1);//database method with argument to save data            
            
        for (int i=0;i<arrayMusic.length;i++){//Loop through array
                 
           if (arrayMusic[i][0] != null){//If data present
                     
                String instrument = arrayMusic[i][0];//Get Instrument
                String musician = arrayMusic[i][1];//Get Musician 
                                               
                sqlString = "INSERT INTO TBL_MUSICIAN (INSTRUMENT, MUSICIAN, SONG_NAME)" //Save to Musician table 
                      + "VALUES ('" + instrument + "','" + musician + "','" + songName + "')";
                
                   getDatabase(1);//database method with argument to save data                      
           }                                    
        } 
        
         //Inform User of Successfull Save
                       Alert alert = new Alert(AlertType.INFORMATION);
                       alert.setTitle("SAVED");
                       alert.setHeaderText("SONG ADDED TO DATABASE");
                       alert.setContentText("Song '" + songName + "' was saved!");
                       alert.show();
       }           
     }
       catch(Exception e){
              e.printStackTrace();
              System.out.println("Error Saving Data");       
        }              
     }    
    
    private void clearFields(){//Method to clear all fields
        
            textName.clear();//Clear Field
            txtComments.clear();           
            radioOG.setSelected(false);
            radioCover.setSelected(false);
            txtLead.clear();//Clear Field
            guitarLead.setSelected(false);
            h1.getChildren().remove(txtLead);//Hide TextBox
            txtLead.clear();
            txtDrums.clear();
            drums.setSelected(false);
            h3.getChildren().remove(txtDrums);//Remove Text Box     
            txtDrums.clear();
            txtBass.clear();
            bass.setSelected(false); 
            h5.getChildren().remove(txtBass);//Hide Text Box   
            txtBass.clear();
            txtRhythm.clear();
            guitarRhythm.setSelected(false);
            h2.getChildren().remove(txtRhythm);//Hide TextBox
            txtRhythm.clear();
            txtVocals.clear();
            vocals.setSelected(false);
            h4.getChildren().remove(txtVocals);//Hide TextBox
            txtVocals.clear();
            txtBanjo.clear();
            banjo.setSelected(false);
            h6.getChildren().remove(txtBanjo);//Hide TextBox
            txtBanjo.clear();    
    }
    
    private ResultSet getDatabase(int num){//Method to connectr to database and execute query or update
        
     ResultSet rs = null;   
        
     try
      {    
          final String DB_URL =//Database Path
            "jdbc:derby://localhost:1527/SongCatalogDB";         
        
         Connection conn = DriverManager.getConnection(DB_URL);// Create a connection to the database.           
               
         if (num == 1){
         Statement stmt = conn.createStatement(); //Create a Statement object.
         stmt.executeUpdate(sqlString);//Insert data into database
         clearFields();
         }
         if (num == 2){
             //System.out.println(sqlString);
             rs = conn.createStatement().executeQuery(sqlString);//Get Data from database   
         }          
      }
      catch(Exception e){
              e.printStackTrace();
              System.out.println("Error on Building Data");             
          }  
     
     return rs;
    }
    
    private void handle(String songSelect) throws SQLException {//Selection Changed Event Handler 
        
         //Tree
         VBox vTree = new VBox();//Box for Tree
         vTree.setPadding(new Insets (40));//Insets
         TreeView<String> treeView = new TreeView<>();
         vTree.getChildren().add(treeView);
         TreeItem<String> root = new TreeItem<String>(songSelect);//Set song as tree root
         treeView.setRoot(root);
         root.setExpanded(true);//Expand tree view
            
         String columnValue1 = "";
         String columnValue2 = "";
         
        try{
         
         sqlString = "SELECT MUSICIAN, INSTRUMENT FROM TBL_MUSICIAN WHERE SONG_NAME = '" + songSelect + "'";//Set SQL string to retrieve details about song
         ResultSet rsTree = getDatabase(2);//Retrieve data for song selected
         
          ResultSetMetaData rsmd = rsTree.getMetaData();
          int columnsNumber = rsmd.getColumnCount();
          while (rsTree.next()) {
                       
          for (int i = 1; i <= columnsNumber; i++) {

              if (i/2==0){
              columnValue1 = rsTree.getString(i); 
              
              }
              if (i/2!=0){
              columnValue2 = rsTree.getString(i); 
              TreeItem<String> add = new TreeItem<String>(columnValue2 + " - " + columnValue1);//Set musician and instrument as tree leaf
              root.getChildren().add(add);
            }               
           }         
          }
        }
          catch(Exception e){
              e.printStackTrace();
              System.out.println("Error Getting Data");             
          } 
     
         Scene treeScene = new Scene(vTree, 500, 500); // This window is a litte bigger since the controls take up more space; watch for clipping if you adjust these numbers!
         Stage newTree = new Stage();
        
         newTree.setTitle("Musicians List for " + songSelect);       // Create window.
         treeScene.getStylesheets().add(getClass().getResource("Final_Style_Sheet.css").toExternalForm());//Invoke CCS
         newTree.setScene(treeScene);
         newTree.show();
    }
    
    
    private boolean validate()//Method to Check that minimum info was entered
    {
        boolean success = true;
        
        //Check for Song Title
        if (textName.getText().trim().isEmpty()==true)
        {          
        Alert alert = new Alert(AlertType.INFORMATION);
        alert.setTitle("Error");
        alert.setHeaderText("No Song Title Entered");
        alert.setContentText("Please Enter a Name for the Song....");
        alert.show();           
        success = false;
        }
        //Check at least 1 Instrument is selected
        else if (guitarLead.isSelected()==false&&drums.isSelected()==false&&
                bass.isSelected()==false&&guitarRhythm.isSelected()==false&&vocals.isSelected()==false&&banjo.isSelected()==false)
        {          
        Alert alert = new Alert(AlertType.INFORMATION);
        alert.setTitle("Error");
        alert.setHeaderText("No Instruments Selected");
        alert.setContentText("Please Select at Least One Instrument & Musician");
        alert.show();       
        success = false;
        }
        //Check That Original or Cover is Selected
        else if (radioCover.isSelected()==false&&radioOG.isSelected()==false)
        {          
        Alert alert = new Alert(AlertType.INFORMATION);
        alert.setTitle("Error");
        alert.setHeaderText("No Song Type Selected");
        alert.setContentText("Please Select Original or Cover....");
        alert.show();     
        success = false;
        }
        
        return success;
    }
     
    public ImageView getImage()//Get Image and return it 
    {     
        //Image imgMusic = Image(Davidson_Final.class.getResource("resource/images/redMusic.jpg"));
        Image imgMusic = new Image("/redMusic.jpg");//Create image object
        ImageView iView = new ImageView(imgMusic);//Create ImageView object             
        iView.setFitWidth(300);
        iView.setFitHeight(75);
        //iView.setPreserveRatio(true);       
        
        return iView;
    }
    public void getAbout()//Method to show box with more info
    {
        Label lblMoreInfo = new Label("fLIP fLOP SONG CATALOG....\n\n--Amber D.  2020");
        Button btnReturn = new Button("Return");
        ImageView img2 = getImage2();//Method to get Header Image      
        VBox hMessage = new VBox(20,lblMoreInfo,img2,btnReturn);//Add all to box
        hMessage.setAlignment(Pos.CENTER);//Set position
                
        Scene informationScene = new Scene(hMessage, 500, 400);   // Set Scene
        Stage information = new Stage();
        information.setTitle("More Info");   // Create window.
        informationScene.getStylesheets().add(getClass().getResource("Final_Style_Sheet.css").toExternalForm());//Invoke CCS 
        information.setScene(informationScene);
        information.show();
        
          btnReturn.setOnAction((ActionEvent event) -> { //Return Button Event Handler 
          information.close();
        });
        
    }
    public ImageView getImage2()//Get Image and return it to scene
    {     
        Image imgMusic = new Image("/smiley.jfif");//Create image object
        ImageView iView = new ImageView(imgMusic);//Create ImageView object             
        iView.setFitWidth(75);
        iView.setFitHeight(75);      
        
        return iView;
    }
    
    private void saveTxt(Stage primaryStage){
        
        FileChooser fchLyrics = new FileChooser();
            fchLyrics.setTitle("Choose Text file for Lyrics");
            fchLyrics.getExtensionFilters().addAll(
                new FileChooser.ExtensionFilter("Text Files", "*.txt")
            );
            fileLyrics = fchLyrics.showOpenDialog(primaryStage);
    }
    
    /**
     * @param args the command line arguments
     */
    public static void main(String[] args) {
        launch(args);
    }

    private Image Image(URL resource) {
        throw new UnsupportedOperationException("Not supported yet."); //To change body of generated methods, choose Tools | Templates.
    }
    
}
