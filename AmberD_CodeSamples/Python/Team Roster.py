# Program to make team roster
#Assignment for class 2022 
# Author:  Amber Davidson
# Date: 3/23/2022


if __name__ == "__main__":

    roster_dict = {}#Declare dictionary  
    

    #FUNCTIONS
    def get_player(inc):
        i = 1
        while i < inc:   # Build original team roster
            if (inc == 2):
                key = int(input("Enter a new player's jersey number:\n"))
                value = int(input("Enter a new player's rating:\n"))
            else:
                print("Enter player " + str(i) +"'s jersey number:")
                key = int(input())
                print("Enter player " + str(i) +"'s rating:")
                value = int(input())
                print()
            roster_dict[key] = value
            i+=1       
        if (inc != 2):
            print_roster()#Print the roster
        
    def print_roster():#Function to print Roster
        print("\nROSTER")
        for x in sorted(roster_dict):
            print("Jersey number: " + str(x) + ", Rating: " + str(roster_dict[x]))

    def print_menu():#Function to print menu
        print("\nMENU")
        print("a - Add player")
        print("d - Remove player")
        print("u - Update player rating")
        print("r - Output players above a rating")
        print("o - Output roster")
        print("q - Quit\n")
        print("Choose an option:")
        
    def remove_player():#Function to remove a player
        key = int(input("Enter a jersey number:\n"))
        roster_dict.pop(key)

    def update_rating():#Function to update player
        key = int(input("Enter a jersey number: \n"))
        value = int(input("Enter a new rating for player: \n"))
        roster_dict[key] = value

    def output_above():
        print("Enter a rating:")
        rating = int(input())
        print("\nABOVE " + str(rating))
        for jersey, rated in sorted(roster_dict.items()):
            if rated > rating:
                print("Jersey number: " + str(jersey) + ", Rating: " + str(rated))

    
    get_player(6)#Get original roster
    #print_roster()#Print the original roster
    print()#Skip a line
    print_menu()#Print the menu
    option = input()#Wait for input

    while (option != "q" and option != "Q"):
        if (option.lower() == "o"):                       
             print_roster()
             print_menu()
             option = input()#Wait for input 

        elif (option.lower() == "a"):
            get_player(2)#Add player
            print_roster()
            print_menu()
            option = input()#Wait for input         

        elif (option.lower() == "d"):
            remove_player()#delete player
            print_roster()
            print_menu()
            option = input()#Wait for input        
            
        elif (option.lower() == "u"):#Update player rating
            update_rating()
            print_roster()
            print_menu()
            option = input()#Wait for input         
           
        elif (option.lower() == "r"):
            output_above()
            #print_roster()
            print_menu()
            option = input()#Wait for input       
          
        elif (option.lower() == "q"):
            exit()


        