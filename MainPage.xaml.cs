namespace MineSweeperGameApp
{
    public partial class MainPage : ContentPage
    {
        private int count = 0;
        private int gameBoardRows = 20;
        private int gameBoardColumns = 20;
        private int totalSquares = 400;
        private int numberOfMines = 50;
        private int numberOfMinesLeft = 50;
        private GameSquare[,] squares2dArray = new GameSquare[20, 20];

        private int screenPixelWidth;

        public MainPage()
        {
            InitializeComponent();
            screenPixelWidth = (int)(DeviceDisplay.Current.MainDisplayInfo.Width * .74);

            string colString, rowString;

            if (GameBoardSizeeEntry.Text.Contains(","))
            {
                string[] dimensions = GameBoardSizeeEntry.ToString().Split(",");
                gameBoardColumns = int.Parse(dimensions[0]);
                gameBoardRows = int.Parse(dimensions[1]);
            }
            else 
            { 
                gameBoardColumns = int.Parse(this.GameBoardSizeeEntry.Text);
                gameBoardRows = int.Parse(this.GameBoardSizeeEntry.Text);
            }

            numberOfMines = int.Parse(this.NumberOfMinesEntry.Text);
            numberOfMinesLeft = numberOfMines;
            this.NumberOfMinesLeftLabel.Text = "Number of mines left: " + numberOfMinesLeft;

            //squares2dArray = new GameSquare[gameBoardRows, gameBoardColumns];
            InitializeGameSquareArray();

            InitializeGameBoard(gameBoardColumns, gameBoardRows);


            TestLabel.Text = GetGameboardTextVersion();
        }

        private void InitializeGameSquareArray()
        {
            squares2dArray = new GameSquare[gameBoardRows, gameBoardColumns];

            for (int i = 0; i < gameBoardRows; i++)
            {
                for (int j = 0; j < gameBoardColumns; j++)
                {
                    squares2dArray[i, j] = new GameSquare();
                    squares2dArray[i, j].Row = i;
                    squares2dArray[i, j].Column = j;
                }
            }
        }

        private string GetGameboardTextVersion()
        {
            string gameboardString = "";

            for (int i = 0; i < gameBoardColumns; i++)
            {
                for (int j = 0; j < gameBoardRows; j++)
                {
                    gameboardString += " ";
                    if (squares2dArray[j, i].IsMined)
                        gameboardString += "#";
                    else if (squares2dArray[j, i].NumberOfAdjacentMines == 0)
                        gameboardString += " ";
                    else gameboardString += squares2dArray[j, i].NumberOfAdjacentMines.ToString();
                }
                gameboardString += Environment.NewLine;
            }
            return gameboardString;
        }
        private void InitializeGameBoard(int numberOfColumns, int numberOfRows)
        {

            int[] minedSquaresArray = GetMinedSquares(numberOfRows * numberOfColumns, numberOfMines);
            GridItemsLayout gameGrid = new GridItemsLayout(numberOfColumns, ItemsLayoutOrientation.Horizontal);

            //this.GameBoardGrid = new Grid();
            this.GameBoardGrid.Clear();
            this.GameBoardGrid.ColumnDefinitions.Clear();
            this.GameBoardGrid.RowDefinitions.Clear();

            // Set mines according to minedSquares array. Must convert flat array indeces to 2d array indeces
            int row = 0;
            int col = 0;
            for (int i = 1; i < minedSquaresArray.Length; i++)
            {
                if (minedSquaresArray[i] == 0) // to avoid divide-by-zero
                {
                    squares2dArray[0, 0].IsMined = true;
                }
                else
                {
                    row = minedSquaresArray[i] / numberOfColumns;
                    col = minedSquaresArray[i] % numberOfColumns;
                    //squares2dArray[row, col] = new GameSquare();
                    squares2dArray[row, col].IsMined = true;
                }
            }

            // Set the number of adjacent mines for each non-mined game square
            for (int i = 0; i < numberOfColumns; i++)
            {
                for (int j = 0; j < numberOfRows; j++)
                {
                    if (squares2dArray[i, j].IsMined) continue;

                    squares2dArray[i, j].NumberOfAdjacentMines = GetNumberOfAdjacentMines(i, j);
                }
            }

            // Setup a grid for the game squares
            int gridColWidth =  screenPixelWidth / (2* gameBoardColumns);
            //int gridColWidth = 20;// screenPixelWidth / (gameBoardColumns / 2);
            int gridRowHeight = gridColWidth;

            for (int i = 0; i < numberOfColumns; i++)
            {
                ColumnDefinition colDefinition = new ColumnDefinition(new GridLength(gridColWidth, GridUnitType.Absolute));
                RowDefinition rowDefinition    = new RowDefinition(new GridLength(gridColWidth, GridUnitType.Absolute));

                this.GameBoardGrid.ColumnDefinitions.Add(colDefinition);
                this.GameBoardGrid.RowDefinitions.Add(rowDefinition);

            }

            // Setup game square buttons

            for (int i = 0; i < numberOfColumns; i++)
            {
                for (int j = 0; j < numberOfRows; j++)
                {
                    squares2dArray[i, j].CornerRadius = 0;
                    squares2dArray[i, j].Padding = 0;
                    squares2dArray[i, j].WidthRequest = gridColWidth;
                    squares2dArray[i, j].HeightRequest = gridColWidth;
                    squares2dArray[i, j].HorizontalOptions = LayoutOptions.Center;
                    squares2dArray[i, j].VerticalOptions = LayoutOptions.Center;
                    squares2dArray[i, j].Source = "minesweeper_unopened_square.svg";

                    //squares2dArray[i, j].Clicked += GameSquare_Clicked;
                    squares2dArray[i, j].Pressed += GameSquare_Pressed;
                    squares2dArray[i, j].Released += GameSquare_Released;

                    //squares2dArray[i, j].Behaviors.a

                    // TODO : Add longpress gesture recognizer
                    //squares2dArray[i, j].GestureRecognizers.

                    this.GameBoardGrid.Add(squares2dArray[i, j], i, j);
                }
            }

        }


        private void GameSquare_Pressed(object sender, EventArgs e)
        {
            ((GameSquare)sender).TimePressed = DateTime.Now;
        }


        private async void GameSquare_Released(object sender, EventArgs e) 
        { 
            GameSquare gameSquare = (GameSquare)sender;
            TimeSpan pressDuration = DateTime.Now.Subtract(gameSquare.TimePressed);

            if( pressDuration.TotalMilliseconds < 750)
            {
                if (gameSquare.IsMined)
                {
                    gameSquare.Source = "exploded_mine.png";

                    // TODO : Show all mines on the gameboard


                    // Game Over 
                    // TODO : Uncomment this. Only commented out to make other testing quicker
                    await DisplayAlert("GAME OVER", "Try again", "OK");
                }
                else if (gameSquare.NumberOfAdjacentMines == 0)
                {
                    // Each game square should have properties for indeces or can they be passed into
                    // this event handler?
                    OpenAdjacentEmptyGameSquares(gameSquare.Row, gameSquare.Column);
                }
                else
                {
                    gameSquare.Source
                        = string.Format("minesweeper_{0}.png", gameSquare.NumberOfAdjacentMines);
                }
            }

            else
            {
                //gameSquare.IsFlagged |= true;
                gameSquare.IsFlagged = !gameSquare.IsFlagged; 
                //gameSquare.Source = gameSquare.IsFlagged ? "minesweeper_flag.png" : "minesweeper_unopened_square.svg";

                if( gameSquare.IsFlagged )
                {
                    gameSquare.Source = "minesweeper_flag.png";
                    this.numberOfMinesLeft--;
                }
                else
                {
                    gameSquare.Source = "minesweeper_unopened_square.svg";
                    this.numberOfMinesLeft++;
                }

                this.NumberOfMinesLeftLabel.Text = "Number of mines left: " + this.numberOfMinesLeft.ToString();

            }

        }


        private async void GameSquare_Clicked(object sender, EventArgs e)
        {
            GameSquare gameSquare = (GameSquare)sender;

            while (gameSquare.IsPressed)
            {
                await Task.Delay(250);
            }

            if ((gameSquare.IsLongPressed) == false)
            { 
                if (gameSquare.IsMined)
                {
                    gameSquare.Source = "exploded_mine.png";

                    // TODO : Show all mines on the gameboard


                    // Game Over 
                    // TODO : Uncomment this. Only commented out to make other testing quicker
                    //await DisplayAlert("GAME OVER", "Try again", "OK");
                }
                else if (gameSquare.NumberOfAdjacentMines == 0)
                {
                    // Each game square should have properties for indeces or can they be passed into
                    // this event handler?
                    OpenAdjacentEmptyGameSquares(gameSquare.Row, gameSquare.Column);
                }
                else
                {
                    gameSquare.Source
                        = string.Format("minesweeper_{0}.png", gameSquare.NumberOfAdjacentMines);

                }
            }
        }

        private void OpenAdjacentEmptyGameSquares(int row, int col)
        {
            // Keep track of visited squares to avoid inifinite backtracking Continuously going back and forth)
            if (squares2dArray[row, col].AlreadyVisited) return;

            for (int i = row - 1; i <= row + 1; i++)
            {
                for (int j = col - 1; j <= col + 1; j++)
                {
                    if (i < 0 || j < 0) continue;
                    if (i > gameBoardRows - 1 || j > gameBoardColumns - 1) continue;
                    if (squares2dArray[i, j] == null) continue;

                    squares2dArray[i, j].Source = string.Format("minesweeper_{0}.png", squares2dArray[i, j].NumberOfAdjacentMines);

                    if ((i == row || j == col) && (squares2dArray[i, j].NumberOfAdjacentMines == 0))
                    {
                        // Mark as visisted to avoid backtracking
                        squares2dArray[row, col].AlreadyVisited = true;
                        OpenAdjacentEmptyGameSquares(i, j);
                    }
                }
            }
        }

        private int[] GetMinedSquares(int totalSquares, int numberOfMines)
        {
            /* 1. Fill an array with all integers from 0 to numberOfMines - 1
             * 2. Shuffle that array
             * 3. Load only the first 50 shuffled integers into the minedSquaresArray.
             *
             *    This will give us an array of 50 random integers between 0 and totalSquares 
             *    that represent indeces into a flat represetation of our 2d squares array
             *    to place the mines.  */

            int[] minedSquaresArray = new int[numberOfMines];
            int[] shuffledSquaresArray = new int[totalSquares];

            var rng = new Random();
            rng.Shuffle(shuffledSquaresArray);

            for (int i = 0; i < totalSquares; i++)
            {
                shuffledSquaresArray[i] = i;
            }

            rng.Shuffle(shuffledSquaresArray);

            for (int i = 0; i < numberOfMines - 1; i++)
            {
                minedSquaresArray[i] = shuffledSquaresArray[i];
            }

            return minedSquaresArray;
        }

        public static void Shuffle(int[] array)
        {
            /* Using Fisher-Yates Algorithm */
            int n = array.Length;
            Random rng = new Random();
            while (n > 1)
            {
                int k = rng.Next(n--);
                int temp = array[n];
                array[n] = array[k];
                array[k] = temp;
            }
        }

        private int GetNumberOfAdjacentMines(int row, int col)
        {
            int adjacentMinesTotal = 0;

            for (int i = row - 1; i <= row + 1; i++)
            {
                if (i < 0 || i > gameBoardRows - 1) continue;

                for (int j = col - 1; j <= col + 1; j++)
                {

                    if (j < 0 || j > gameBoardColumns - 1 || !squares2dArray[i, j].IsMined) continue;

                    adjacentMinesTotal++;

                }
            }

            return adjacentMinesTotal;
        }

        private void OnCounterClicked(object sender, EventArgs e)
        {
            UncoverAllSquares();
        }

        private void UncoverAllSquares()
        {
            for (int i = 0; i < gameBoardRows; i++)
                for (int j = 0; j < gameBoardColumns; j++)
                    if (squares2dArray[i, j].IsMined)
                        squares2dArray[i, j].Source = "gnome_gnomine.png";
                    else
                        squares2dArray[i, j].Source = string.Format("minesweeper_{0}.png", squares2dArray[i, j].NumberOfAdjacentMines);

        }

        private void NewGameButton_Clicked(object sender, EventArgs e)
        {
            gameBoardColumns = int.Parse(this.GameBoardSizeeEntry.Text);
            gameBoardRows = int.Parse(this.GameBoardSizeeEntry.Text);
            numberOfMines = int.Parse(this.NumberOfMinesEntry.Text);
            numberOfMinesLeft = numberOfMines;

            InitializeGameSquareArray();
            InitializeGameBoard(gameBoardRows, gameBoardColumns);
            TestLabel.Text = GetGameboardTextVersion();
        }

        private void HideCheatPanelButton_Clicked(object sender, EventArgs e)
        {
            this.TestLabel.IsVisible = !this.TestLabel.IsVisible;
            if (this.TestLabel.IsVisible)
                this.HideCheatPanelButton.Text = "Hide Cheat Sheet";
            else
                this.HideCheatPanelButton.Text = "Show Cheat Sheet";
        }

        private void SaveGameSetup_Clicked(object sender, EventArgs e)
        {

        }
    }


}
