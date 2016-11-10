using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Minefield
{
    public partial class Form1 : Form
    {
        int atX = 10;
        int atY = 20;

        int mineCount = 20;
        int[] mineLocations;

        bool[] visitedLocations;

        int locationsCleared;

        bool playerDead;

        public Form1()
        {
            InitializeComponent();            
            StartNewGame();            
        }

        void StartNewGame()
        {
            visitedLocations = new bool[400];                      
            SetLocationImages();
            PlacePlayerAtStart();
            RandomlyPlaceBombs();
            PutPlayerOnLabel();
            playerDead = false;
        }

        void PlacePlayerAtStart()
        {
            //Set the player's start position
            atX = 10;
            atY = 20;
        }

        void SetLocationImages()
        {
            //Go through all the locations
            for(int i = 0; i < 400; i++)
            {
                //Get the label name
                string labelName = "label" + (i + 1).ToString();
                //Find the label
                Label label = this.Controls.Find(labelName, true).FirstOrDefault() as Label;
                //Set the label image to grass
                label.Image = Properties.Resources.grass;
            }

            locationsCleared = 0;
        }

        void ShowLocationsCleared()
        {            
            //Get the percentage of locations that have been visited (take out the locations of the mines because we cant visit them)
            float percent = ((float)locationsCleared / (400 - mineCount)) * 100; //Convert to float or it always comes back as 0

            //Make it an int again so it's readable on screen
            int percentInt = (int)percent;

            //Update the label
            clearedText.Text = percentInt.ToString() + "% Cleared";

            //Check to see if all non-mine locations have been checked
            if( (400 - ((locationsCleared + 1) + mineCount) ) == 0) //plus 1 because the number of locations visited id counted after they've been visited
            {
                //if so show the you win message box
                YouWin();
            }
        }

        void YouWin()
        {
            //Show a congratulations messagebox            
            DialogResult result = MessageBox.Show("Congratulations, you have successfully traversed the minefield.", "You Win!", MessageBoxButtons.OK);

            //If the button is clicked start a new game
            //http://stackoverflow.com/questions/16334323/event-handlers-on-message-box-buttons
            if (result == DialogResult.OK)
            {
                StartNewGame();
            }
        }

        void RandomlyPlaceBombs()
        {
            //set the mine location array to be the number of mines
            mineLocations = new int[mineCount];

            //Create a new randomiser
            //https://www.dotnetperls.com/random
            Random rand = new Random();

            for (int i = 0; i < mineCount; i++)
            {
                //If the random location is the same as the start locatio or an already listed location then just create a new random                
                int location;
                do
                {
                    //get a random location
                    location = GetRandomLocation(rand);
                }
                while (CheckLocation(location) == true); //Check it's a valid location

                //Add the location to the mine location array
                mineLocations[i] = location;                
            }
        }

        bool CheckLocation(int location)
        {            
            for(int i = 0; i < mineLocations.Length; i++)
            {
                //Is this location already in the location array?
                if (mineLocations[i] == location)
                {
                    //If so get a different location
                    return true;
                }
            }

            //Is this location the same as the start location?
            int startLocation = ((atY - 1) * 20) + atX;
            if(startLocation == location)
            {
                //If so get a different location
                return true;
            }

            //If neither then this number is ok
            return false;
        }

        int GetRandomLocation(Random rand)
        {
            //Get a random number between 1 and 400
            return rand.Next(1, 400);
        }

        string GetLabelFromLocation()
        {            
            return "label" + (((atY-1) * 20) + atX).ToString();
        }

        public void PutPlayerOnLabel()
        {            
            //Find the label where the player is
            Label label = this.Controls.Find(GetLabelFromLocation(), true).FirstOrDefault() as Label;            

            //Put the player sprite on the label
            label.Image = Properties.Resources.tank;
            
            //Update the % cleared
            ShowLocationsCleared();

            //Check if any mines are adjacent
            CheckForMines();
        }

        void HaveBeenHere()
        {
            //Get the player's location
            int location = ((atY - 1) * 20) + atX;
            
            //Check if the player has been here before
            bool beenHere = visitedLocations[location - 1]; //0 -> 399 not 1 -> 400           

            //if not then...
            if (beenHere == false)
            {
                //Mark the player as having visited this location
                visitedLocations[location - 1] = true;
                //increase the cleared locations counter
                locationsCleared++;
            }

            //Update the label's image to show that this location is empty
            Label label = this.Controls.Find(GetLabelFromLocation(), true).FirstOrDefault() as Label;
            label.Image = Properties.Resources.tanMud;
        }

        void CheckForMines()
        {
            int mineCounter = 0;

            //is there a mine above?
            if (IsMineAbove())
            {
                //if so add 1 to the counter
                mineCounter++;
            }

            //Below
            if (IsMineBelow())
            {
                mineCounter++;
            }

            //Left
            if (IsMineLeft())
            {
                mineCounter++;
            }

            //Right
            if (IsMineRight())
            {
                mineCounter++;
            }

            //Show haw many mines are adjacent
            radarText.Text = mineCounter.ToString();
        }

        bool IsMineAbove()
        {
            //Compensate for 0 indexing            
            int y = atY - 1;

            //If atY == 1 the we are at the top of the map
            if (atY == 1)
            {
                return false;
            }

            //Get the label location above the current location
            int aboveLocation = ((y - 1) * 20) + atX;

            //Check if that location is in the array of mine locations
            for(int i = 0; i < mineLocations.Length; i++)
            {
                //The location above is in the mine locations
                if(mineLocations[i] == aboveLocation)
                {
                    //There is a mine above
                    return true;
                }
            }

            //Otherwise, there isn't a mine above
            return false;
        }

        bool IsMineLeft()
        {
            //Compensate for 0 indexing            
            int y = atY - 1;

            //If atX == 1 the we are at the left side of the map
            if (atX == 1)
            {
                return false;
            }

            //Get the label location to the left of the current location
            int aboveLocation = (y * 20) + (atX - 1);

            //Check if that location is in the array of mine locations
            for (int i = 0; i < mineLocations.Length; i++)
            {
                //The location to the left is in the mine locations
                if (mineLocations[i] == aboveLocation)
                {
                    //There is a mine left
                    return true;
                }
            }

            //Otherwise, there isn't a mine to the left
            return false;
        }

        bool IsMineRight()
        {
            //Compensate for 0 indexing            
            int y = atY - 1;

            //If atX == 20 the we are at the right side of the map
            if (atX == 20)
            {
                return false;
            }

            //Get the label location to the right of the current location
            int aboveLocation = (y * 20) + (atX + 1);

            //Check if that location is in the array of mine locations
            for (int i = 0; i < mineLocations.Length; i++)
            {
                //The location to the right is in the mine locations
                if (mineLocations[i] == aboveLocation)
                {
                    //There is a mine right
                    return true;
                }
            }

            //Otherwise, there isn't a mine right
            return false;
        }

        bool IsMineBelow()
        {
            //If atY == 20 the we are at the top of the map
            if (atY == 20)
            {
                return false;
            }

            //Compensate for 0 indexing            
            int y = atY - 1;

            //Get the label location below the current location
            int aboveLocation = ((y + 1) * 20) + atX;

            //Check if that location is in the array of mine locations
            for (int i = 0; i < mineLocations.Length; i++)
            {
                //The location below is in the mine locations
                if (mineLocations[i] == aboveLocation)
                {
                    //There is a mine below
                    return true;
                }
            }

            //Otherwise, there isn't a mine below
            return false;
        }

        bool IsMineActivated(int x, int y)
        {
            //Compensate for 0 indexing
            y = y - 1;

            //Get the label location of the destination
            int destination = (y * 20) + x;

            //Check the array of mines to see if their loaction is equal to the player's current location
            for (int i = 0; i < mineLocations.Length; i++)
            {                
                if(mineLocations[i] == destination)
                {
                    //if so we've set off a land mine
                    return true;
                }
            }

            //otherwise no landmine has been set off
            return false;
        }

        void ExplodeMine()
        {
            //Set the player to dead so that they can't move anymore
            playerDead = true;

            //Set all the loactions as empty
            for(int i = 0; i < 400; i++)
            {
                string displayValue = "label" + (i + 1).ToString();

                Label l = this.Controls.Find(displayValue, true).FirstOrDefault() as Label;
                l.Image = Properties.Resources.tanMud;
            }

            //Show the mines
            ShowAllMines();

            //Explode the mine that the player set off
            Label label = this.Controls.Find(GetLabelFromLocation(), true).FirstOrDefault() as Label;
            label.Image = Properties.Resources.boom;

            //Show a congratulations messagebox            
            DialogResult result = MessageBox.Show("Unfortunately you triggered a mine.", "You Lose :(", MessageBoxButtons.OK);

            //If the button is clicked start a new game
            //http://stackoverflow.com/questions/16334323/event-handlers-on-message-box-buttons
            if (result == DialogResult.OK)
            {
                StartNewGame();
            }
        }

        void ShowAllMines()
        {
            //Display all mines
            for (int i = 0; i < mineLocations.Length; i++)
            {
                Label l = this.Controls.Find("label" + mineLocations[i].ToString(), true).FirstOrDefault() as Label;
                l.Image = Properties.Resources.mine;
            }
        }

        private void MoveLeft(object sender, EventArgs e)
        {
            //Only move if we aren't dead
            if (playerDead == false)
            {
                //Only move if we aren't at the edge of the map
                if (atX > 1)
                {
                    //Change the ground label image to show that label has been visited
                    HaveBeenHere();

                    //Check if a mine has been set off
                    if (IsMineActivated(atX - 1, atY) == false)
                    {
                        //Update the player's position
                        atX--;

                        //Show the player
                        PutPlayerOnLabel();
                    }
                    else
                    {
                        //Move the player's location onto the mine
                        atX--;

                        //Mine has been set off
                        ExplodeMine();
                    }
                }
            }           
        }

        private void MoveRight(object sender, EventArgs e)
        {
            //Only move if we aren't dead
            if (playerDead == false)
            {
                //Only move if we aren't at the edge of the map
                if (atX < 20)
                {
                    //Change the ground label image to show that label has been visited
                    HaveBeenHere();


                    if (IsMineActivated(atX + 1, atY) == false)
                    {
                        //Update the player's position
                        atX++;

                        //Show the player
                        PutPlayerOnLabel();
                    }
                    else
                    {
                        //Move the player's location onto the mine
                        atX++;

                        //Mine has been set off
                        ExplodeMine();
                    }
                }
            }         
        }

        private void MoveUp(object sender, EventArgs e)
        {
            //Only move if we aren't dead
            if (playerDead == false)
            {
                //Only move if we aren't at the edge of the map
                if (atY > 1)
                {
                    //Change the ground label image to show that label has been visited
                    HaveBeenHere();
                    if (IsMineActivated(atX, atY - 1) == false)
                    {
                        //Update the player's position
                        atY--;

                        //Show the player
                        PutPlayerOnLabel();
                    }
                    else
                    {
                        //Move the player's location onto the mine
                        atY--;

                        //Mine has been set off
                        ExplodeMine();
                    }
                }
            }         
        }

        private void MoveDown(object sender, EventArgs e)
        {
            //Only move if we aren't dead
            if (playerDead == false)
            {
                //Only move if we aren't at the edge of the map
                if (atY < 20)
                {
                    //Change the ground label image to show that label has been visited
                    HaveBeenHere();
                    if (IsMineActivated(atX, atY + 1) == false)
                    {
                        //Update the player's position
                        atY++;

                        //Show the player
                        PutPlayerOnLabel();
                    }
                    else
                    {
                        //Move the player's location onto the mine
                        atY++;

                        //Mine has been set off
                        ExplodeMine();
                    }
                }
            }         
        }

        private void startNewGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Start a new game
            StartNewGame();
        }    

        private void Difficulty_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Check which difficulty was selected, set the mine count to the correct number for the difficulty and then start a new game
            if(Difficulty.SelectedIndex == 0)
            {
                mineCount = 10;
                StartNewGame();
            }

            if (Difficulty.SelectedIndex == 1)
            {
                mineCount = 20;
                StartNewGame();
            }

            if (Difficulty.SelectedIndex == 2)
            {
                mineCount = 30;
                StartNewGame();
            }            
        }        

        private void cheatModeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Show the mines
            ShowAllMines();            
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Set the difficulty based on how many mines there are
            if (mineCount == 10)
            {
                Difficulty.SelectedIndex = 0;
            }

            if (mineCount == 20)
            {
                Difficulty.SelectedIndex = 1;
            }

            if (mineCount == 30)
            {
                Difficulty.SelectedIndex = 2;
            }
        }
    }    
}
