/*                                
Amber Davidson
2/9/2020
This program opens a picture and adjusts it
Developed and compiled using Netbeans 8.2
 */
package davidson_color_adjuster;

import java.io.File;
import javafx.application.Application;
import javafx.event.ActionEvent;
import javafx.geometry.Pos;
import javafx.scene.Scene;
import javafx.scene.control.Button;
import javafx.scene.control.Label;
import javafx.scene.control.Slider;
import javafx.scene.image.Image;
import javafx.scene.image.ImageView;
import javafx.scene.layout.VBox;
import javafx.stage.Stage;
import javafx.stage.FileChooser;
import javafx.scene.effect.ColorAdjust;
import java.awt.image.BufferedImage;
import java.io.IOException;
import javafx.embed.swing.SwingFXUtils;
import javax.imageio.ImageIO;
import javafx.geometry.Insets;



public class Davidson_Color_Adjuster extends Application {
    //Global Variables
        //Labels
        final Label lblHeader = new Label("Choose picture (.jpg), then adjust...");//User Instruction      
        final Label lblHue = new Label("Hue:");
        final Label lblSat = new Label("Saturation:");
        final Label lblBright = new Label("Brightness:");
        final Label lblContrast = new Label("Contrast:");
        Label lblError = new Label();//Errorm Msg       
        //Buttons
        Button btnPic = new Button("Open Picture");//Button to open pic
        Button btnSave = new Button("Save");//Button to save  
        //VBoxes
        VBox show = new VBox(15);//HBox for Image
        VBox sliders = new VBox(5);
        //Image 
        ImageView img = new ImageView();//View Image
        String fileName1 = "c:/temp/1.jpg"; //For Saving
    //End Variables
      
   @Override
    public void start(Stage primaryStage) {
            
        VBox mainBox = getComponents();//Methods for sliders & VBoxes            
        FileChooser fileChooser = new FileChooser();//Instantiate File Chooser

        //Get Picture Button Click Event
        btnPic.setOnAction((ActionEvent event) -> {       
            File selectedFile = fileChooser.showOpenDialog(primaryStage);//Choose
            if (selectedFile != null)//Check file selected
            {
            getImage(selectedFile);//Methods that gets image and sets ImageView      
            }
            else
            lblError.setText("No File Selected");
        });   
              
       btnSave.setOnAction((ActionEvent event) -> {//DOES NOT SAVE FILE CORRECTLY
           File outputFile = new File("imageViewAdjusted");
           BufferedImage bImage = SwingFXUtils.fromFXImage(img.snapshot(null, null), null);
           try {
                File saveFile = fileChooser.showSaveDialog(primaryStage);//Choose Box
                //ExtensionFilter filter = new FileChooser.ExtensionFilter("JPEG", "jpg");
                ImageIO.write(bImage, "jpeg", outputFile);
           } catch (IOException e) {
               throw new RuntimeException(e);
           }
        });
        
        //Set and Show Scene
        Scene scene = new Scene(mainBox,600,775);
        scene.getStylesheets().add(getClass().getResource("Color_Adjust.css").toExternalForm());//Invoke CCS
        primaryStage.setTitle("Color Adjuster");
        primaryStage.setScene(scene);//Put scene in stage
        primaryStage.show();//Show stage
    }

    public VBox getComponents()
    {
        //Sliders
        Slider sldHue  = new Slider(-1,1,0);//Hue Slider          
        sldHue.setShowTickLabels(true);//Show Labels
        sldHue.setMajorTickUnit(.25);//Label Increments
        Slider sldSat  = new Slider(-1,1,0);//Saturation Slider
        sldSat.setShowTickLabels(true);//Show Labels
        sldSat.setMajorTickUnit(.25);//Label Increments
        Slider sldBright  = new Slider(-1,1,0);//Brightness Slider
        sldBright.setShowTickLabels(true);//Show Labels
        sldBright.setMajorTickUnit(.25);//Label Increments
        Slider sldCon  = new Slider(-1,1,0);//Contrast Slider
        sldCon.setShowTickLabels(true);//Show Labels
        sldCon.setMajorTickUnit(.25);//Label Increments
        return setBoxes(sldHue,sldSat,sldBright,sldCon);        
    }
    
    public VBox setBoxes(Slider sldHue,Slider sldSat,Slider sldBright,Slider sldCon)
    {
        VBox top = new VBox(10,lblHeader,btnPic);//VBox for header and button
        top.setAlignment(Pos.BOTTOM_CENTER);//Set Alignment              
        VBox main = new VBox(10,top,lblError,show);//VBox for all components
        main.setAlignment(Pos.CENTER);//Position Main VBox
        getAdjustments(sldHue,sldSat,sldBright,sldCon);//Call method to get slider adjustments
        sliders.getChildren().addAll(lblHue,sldHue,lblSat,sldSat,lblBright,sldBright,lblContrast,sldCon);//Add Sliders
        sliders.setPadding(new Insets(20));//Padding
        return main;//Return mian VBox with all controls 
    }
    
    public void getImage(File selectedFile)
    {                    
        Image picture = new Image(selectedFile.toURI().toString());//Set file as Image
        img.setImage(picture);//Set as Image
        img.setFitWidth(300);//Set Image Width
        img.setFitHeight(300);//Set Image Height         
        lblError.setText("");//Clear Eror Msg   
        show.getChildren().clear();//Clear previous image
        show.getChildren().addAll(img,sliders,btnSave);//Add image,save, and sliders to VBox    
        show.setAlignment(Pos.CENTER);//Set Alignment     
        ImageView imageViewAdjusted = new ImageView(new Image(getClass().getResource("img.jpg").toExternalForm(), 250, 250, true, true));
    }
  
    public void getAdjustments(Slider sldHue,Slider sldSat,Slider sldBright,Slider sldCon)
    {      
        ColorAdjust colorAdjust = new ColorAdjust();//Instantiate ColorAdjust
        
        //Hue Adjustment Listener
        sldHue.valueProperty().addListener((obsevable) -> {           
        colorAdjust.setHue(sldHue.getValue());
        });       
        //Saturation Adjustment Listener
        sldSat.valueProperty().addListener((observable) -> {           
        colorAdjust.setSaturation(sldSat.getValue());
        });
        //Brightness Adjustment Listener
        sldBright.valueProperty().addListener((observable) -> {           
        colorAdjust.setBrightness(sldBright.getValue());
        });
        //Contrast Adjustment Listener
        sldCon.valueProperty().addListener((observable) -> {           
        colorAdjust.setContrast(sldCon.getValue());
        });
        
        img.setEffect(colorAdjust);//Apply adjustments to image
    }
  
    public static void main(String[] args) {
        launch(args);
    }
    
}
