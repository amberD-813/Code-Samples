/*                                
Amber Davidson
3/18/2020
This program gets location of string in array with generic class
Developed and compiled using Netbeans 8.2
 */
package davidson_generics;

import javafx.application.Application;
import static javafx.application.Application.launch;
import javafx.event.ActionEvent;
import javafx.geometry.Pos;
import javafx.scene.Scene;
import javafx.scene.control.Button;
import javafx.scene.control.Label;
import javafx.scene.control.TextArea;
import javafx.scene.control.TextField;
import javafx.scene.layout.VBox;
import javafx.scene.layout.HBox;
import javafx.stage.Stage;
import javafx.geometry.Insets;

public class Davidson_Generics extends Application {
    
    //Global Variables
    VBox vMain = new VBox(30);//Main Scene Box
    HBox hText = new HBox(10);
    VBox vFields = new VBox(20);
    TextArea txtEntry = new TextArea();//Box for user entry
    TextField txtSearch = new TextField();//Box for user entry
    static String[] stringArray = new String[25];//Array to hold strings
    String sentenceString;//Holds sentance input
    static String word;
    Button btnSearch = new Button("Search");//Button to start search 
    Button btnEnter = new Button("Enter");//Button to start search5
    Label instruct = new Label(); 
    Label lblError = new Label("Invalid Entry");
    Label lblDisplay = new Label();
    static int amount;//Holds number of times string is found
    String lower;
    
    @Override
    public void start(Stage primaryStage) {     
        setUp();//Set up Entry Screen
    
        btnEnter.setOnAction((ActionEvent event) -> {
              getString1();//Get first Input String for sentence  
        } 
        );
        btnSearch.setOnAction((ActionEvent event) -> {    
             getString2(); //Get second input string for word and analyze           
        } 
        );
        
        Scene scene = new Scene(vMain, 650, 750);  
        scene.getStylesheets().add(getClass().getResource("stringSearch.css").toExternalForm());//Invoke CCS
        primaryStage.setTitle("Generics");
        primaryStage.setScene(scene);
        primaryStage.show();
    }
    private void  setUp(){//Set up first box 
        vMain.setPadding(new Insets(15));
        txtEntry.setId("txtEntry");
        txtEntry.setWrapText(true);
        vFields.setAlignment(Pos.CENTER);//Position field box          
        hText.getChildren().add(txtEntry);
        hText.setAlignment(Pos.CENTER);//Position text box
        instruct.setText("Type a sentence or paragraph...\n           Then press Enter.");
        instruct.setId("txtInstruct");
        vMain.setAlignment(Pos.CENTER);//Position main box
        vFields.getChildren().addAll(instruct,hText,btnEnter);//Add buttong to main box
        vMain.getChildren().add(vFields);//put all fields in main box
    }
    private void getString1(){
        try{
           sentenceString = txtEntry.getText();//Attempt to get string
           populateArray();//Method to Load Array
           lblDisplay.setText(sentenceString);//Display what was entered
           if(stringArray.length>2){
             setNewBoxes();//Set up next screen
           }
           }
         catch(Exception e){
           lblError.setText("Invalid Entry");//Set error message
           vMain.getChildren().addAll(lblError);//Show Error
       }       
    }
    private void getString2(){
         try{
           word = txtSearch.getText().trim();//Attempt to get word
           
           ObjectBinarySearch.searchGeneric(stringArray);//Generic Class
           
           if(amount>0)
           {
               vMain.getChildren().remove(instruct);
               vMain.getChildren().remove(btnSearch);//Remove Search Button
               vMain.getChildren().remove(hText);//Remove text box
               Label lblDisplay2 = new Label();
               vMain.getChildren().add(lblDisplay2);
               lblDisplay2.setText("\n\nThe word '"+word+"' was found "+amount+" times.\n"+
                                  "            Big O Analysis = O(n)");                        
           }
         }
         catch(Exception e){
           lblError.setText("Invalid Entry");//Set error message
           vMain.getChildren().addAll(lblError);//Show Error
         }                     
        
    }
    public void setNewBoxes(){
       hText.getChildren().remove(txtEntry);
       hText.getChildren().add(txtSearch);
       vMain.getChildren().remove(vFields);
       lblDisplay.setText("'"+sentenceString+"'");//Display sentence for reference
       lblDisplay.setWrapText(true);
       instruct.setText("Type a word, then click search to find \nhow many times that word occurs in:");
       vMain.getChildren().addAll(instruct,lblDisplay,hText,btnSearch);     
    }
        
   private void populateArray(){//This method populates a string array with the sentence provided
       lower = sentenceString.toLowerCase();//Make a lower for comparison
       stringArray = lower.split(" ");//Populate array with each word
    }
    
   public static class ObjectBinarySearch<T extends Comparable<T>>{      
       
       Object[] arr;//Not currently used
        public static void searchGeneric(Comparable[] array){
               
            for (Comparable array1 : array) {//Enhanced sequential loop
                
                if (array1.toString().equals(word)||array1.toString().equals(word +".")) {//Make method find both string or int depending on which is sent.
                
                    amount++; //Count up                
                }
            }                  
       }      
    }
    /**
     * @param args the command line arguments
     */
    public static void main(String[] args) {
        launch(args);
    }
    
}
