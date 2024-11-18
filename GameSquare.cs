using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Behaviors;

namespace MineSweeperGameApp
{
    internal class GameSquare : ImageButton
    {
        private int numberOfAdjacentMines;
        private int column;
        private int row;
        private bool isUncovered;
        private bool isFlagged;
        private bool isMined;
        private bool isQuestioned;
        private bool alreadyVisited;
        private bool isLongPressed;
        private DateTime timePressed;
        public Command LongPressCommand { get; set; }
        public Command ShortPressCommand { get; set; }

        public GameSquare()
        {           
            // Can use numberOfAdjacentMines to designate a mine using -1 or 9 or similar
            numberOfAdjacentMines = 0;
            column = 0;
            row = 0;
            isUncovered = false;
            isFlagged = false;
            isMined = false;
            alreadyVisited = false;
            isLongPressed = false;
            CornerRadius = 0;           
        }


        public int NumberOfAdjacentMines { get => numberOfAdjacentMines; set => numberOfAdjacentMines = value; }
        public bool IsUncovered { get => isUncovered; set => isUncovered = value; }
        public bool IsFlagged { get => isFlagged; set => isFlagged = value; }
        public bool IsMined { get => isMined; set => isMined = value; }
        public bool IsQuestioned { get => isQuestioned; set => isQuestioned = value; }
        public int Column { get => column; set => column = value; }
        public int Row { get => row; set => row = value; }
        public bool AlreadyVisited { get => alreadyVisited; set => alreadyVisited = value; }
        public bool IsLongPressed { get => isLongPressed; set => isLongPressed = value; }
        public DateTime TimePressed { get => timePressed; set => timePressed = value; }
        public bool IsFlagged1 { get => isFlagged; set => isFlagged = value; }
        //        public Button Button { get => button; set => button = value; }


    }
}
