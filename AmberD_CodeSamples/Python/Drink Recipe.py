"""Author : Amber Davidson
   Date: 2/3/2022
   File Name: drink_recipe
"""

#Get input for recipe
lemonJ = float(input("Enter amount of lemon juice (in cups):" + "\n"))
water = float(input("Enter amount of water (in cups):" + "\n"))
agaveN = float(input("Enter amount of agave nectar (in cups):" + "\n"))
servings = float(input("How many servings does this make?\n"))
units = 'cup'

def printLoop(units): 
    print('\nLemonade ingredients - yields ' + (f'{servings:.2f}') + ' servings')
    print((f'{lemonJ:.2f} ') + units + ' (s) lemon juice')
    print((f'{water:.2f} ') + units + ' (s) water')
    print((f'{agaveN:.2f} ') + units + ' (s) agave nectar')

#Reiterate input
printLoop(units)

#Get serving amount to make
servingsToMake = float(input('\nHow many servings would you like to make?\n'))

#Calculate new recipe
offset = servingsToMake / servings
servings = servingsToMake
lemonJ *= offset
water *= offset
agaveN *= offset

#Show recipe
printLoop(units)

#Calculate gallons
offset = 16
lemonJ /= offset
water /= offset
agaveN /= offset
units = 'gallon'

#Show recipe
printLoop(units)