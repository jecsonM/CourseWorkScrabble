using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ScrabbleLib.Scrabble;

namespace ScrabbleLib
{
    public class Scrabble //Класс игры Эрудит
    {
        const int MAX_WIDTH_OF_GAME_BOARD = 150;
        const int MAX_HEIGTH_OF_GAME_BOARD = 150;
        const int MAX_AMOUNT_OF_EACH_LETTER_IN_HEAP = 2500;

        public class Letter //Буквы для игры их на доску
        {
            public char character { get; }
            public int x = -1;
            public int y = -1;
            public Letter up = null;
            public Letter right = null;
            public Letter down = null;
            public Letter left = null;
            
            public Letter(char character)
            {
                this.character = character; 
            }
            Letter this[int i]
            {
                get {
                    switch (i)
                    {
                        case 0:
                            return up;
                        case 1:
                            return right;
                        case 2:
                            return down;
                        case 3:
                            return left;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
        }
        public class GameBoard //Игровая доска
        {
            public int GetLinearPosition(Letter letter)
            {
                return width * letter.y + letter.x;
            }
            public int GetLinearPosition(int x, int y)
            {
                return width * y + x;
            }
            public Dictionary<int, Letter> gameBoard { get; }
            public int width { get; }
            public int heigth { get; }
            
            GameBoard(HeapOfCharacters heapOfLetters, int width, int height)
            {
                if (width <= 0 || width > MAX_WIDTH_OF_GAME_BOARD) throw new Exception($"Width must be between 0 and {MAX_WIDTH_OF_GAME_BOARD}");
                if (heigth <= 0 || heigth > MAX_HEIGTH_OF_GAME_BOARD) throw new Exception($"Heigth must be between 0 and {MAX_HEIGTH_OF_GAME_BOARD}");
                this.width = width;
                this.heigth = height;
                gameBoard = new Dictionary<int, Letter>(heapOfLetters.totalAmountOfCharacters);
            }

            
            public bool TryToPlaceLetter(Letter letter) //Попытаться разместить букву. True, если удалось
            {
                int letterLinearPosition = GetLinearPosition(letter);
                if (gameBoard.ContainsKey(letterLinearPosition) || letter.x >= width || letter.y >= heigth)
                    return false;
                gameBoard[letterLinearPosition] = letter;
                /////////////Сделать подвязку со стороны соседних букв
                fastLetterSideLinking(letter);
                return true;
            }
            public void fastLetterSideLinking(Letter letter)
            {
                int letterLinearPosition = GetLinearPosition(letter);

                letter.right = GetLetterAt(letterLinearPosition + 1);
                if (letter.right != null) letter.right.left = letter;

                letter.left = GetLetterAt(letterLinearPosition - 1);
                if (letter.left != null) letter.left.right = letter;

                letter.up = GetLetterAt(letterLinearPosition + width);
                if (letter.up != null) letter.up.down = letter;

                letter.down = GetLetterAt(letterLinearPosition - width);
                if (letter.down != null) letter.down.up = letter;
            }
            private Letter GetLetterAt(int letterLinearPosition) //Получить букву с доски из хэша буквы
            {
                
                if (gameBoard.ContainsKey(letterLinearPosition))
                {
                    return gameBoard[letterLinearPosition];
                }
                return null;
            }
            
        }

        public class HeapOfCharacters //Куча букв из которых набирается рука
        {
            public Dictionary<char, int> characters { get; } //словарь букв их количеств в кучке
            public int totalAmountOfCharacters { get; }
            public int currentAmountOfCharacters { get; private set; }
            HeapOfCharacters(Dictionary<char, int> characters)
            {
                this.characters = characters;
                totalAmountOfCharacters = 0;
                foreach (KeyValuePair<char, int> kvp in this.characters)
                {
                    if (kvp.Key == '#')
                        throw new Exception("Symbol # is being used as a sign of error and can't be one of the letters");
                    if (kvp.Value < 0 || kvp.Value > MAX_AMOUNT_OF_EACH_LETTER_IN_HEAP)
                        throw new Exception($"Letters ammount must be between 0 and {MAX_AMOUNT_OF_EACH_LETTER_IN_HEAP}");
                    totalAmountOfCharacters += kvp.Value;
                }
                if (totalAmountOfCharacters == 0) throw new Exception("There are must be at least one letter");
                currentAmountOfCharacters = totalAmountOfCharacters;
            }

            public bool CheckAmIOnlyRussian()
            {
                foreach (KeyValuePair<char, int> kvp in characters)
                    if (kvp.Key < 'а' || kvp.Key > 'я' && kvp.Key != '*') //Если не русские символи и не *, то не только русские
                        return false;
                return true;
            }
            public char ExtractRandomChar()
            {
                if (currentAmountOfCharacters <= 0) return '#';
                int randLetterPos = new Random().Next(0,currentAmountOfCharacters);
                int curretntLetterPos = 0;
                Dictionary<char, int>.KeyCollection letterCharacters = characters.Keys;
                foreach ( char character in letterCharacters)
                {
                    curretntLetterPos += characters[character];
                    if (curretntLetterPos <= randLetterPos)
                    {
                        characters[character]--;
                        currentAmountOfCharacters--;
                        return character;
                    }
                }
                return '#'; //ошибочная буква
            }

        }
        public class Hand //Рука с буквами, выкладываемые на доску
        {
            public enum LetterStatus 
            {
                InHand = 0,
                OnBoard,
                FinishedPlacing,
                Empty
            }
            
            public HeapOfCharacters heapOfLetters { get; }
            public GameBoard gameBoard { get; }
            public LetterStatus[] letterStatuses { get; }
            Letter[] letters;
            
            Hand(GameBoard gameBoard, HeapOfCharacters heapOfLetters, int handCapacity)
            {
                this.gameBoard = gameBoard;
                this.heapOfLetters = heapOfLetters;
                this.letterStatuses = new LetterStatus[handCapacity];
                this.letters = new Letter[handCapacity];
            }
            public void Reroll()
            {
                for (int i = 0; i < letters.Length; i++)
                {
                    letterStatuses[i] = LetterStatus.InHand;
                    letters[i] = new Letter(heapOfLetters.ExtractRandomChar());
                    if (letters[i].character == '#') letterStatuses[i] = LetterStatus.Empty;
                }
            }
        }

    }
   
    
}
