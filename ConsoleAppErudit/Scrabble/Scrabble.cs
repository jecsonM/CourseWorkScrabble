using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scrabble
{
    public class Scrabble //Класс игры Эрудит
    {
        const int MAX_WIDTH_OF_GAME_BOARD = 150;
        const int MAX_HEIGTH_OF_GAME_BOARD = 150;
        const int MAX_AMOUNT_OF_EACH_LETTER_IN_HEAP = 2500;

        public class Letter //Буквы для игры их на доску
        {
            char letter;
            public int x = -1;
            public int y = -1;
            public Letter up = null;
            public Letter right = null;
            public Letter down = null;
            public Letter left = null;
            Letter this[int i]
            {
                get 
                {
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
            public Dictionary<int, Letter> gameBoard { get; }
            public int width { get; }
            public int heigth { get; }
            int GetLettersHashCode(Letter letter)
            {
                return this.width * letter.y + letter.x;
            }
            GameBoard(HeapOfLetters heapOfLetters, int width, int height)
            {
                if (width <= 0 || width > MAX_WIDTH_OF_GAME_BOARD) throw new Exception($"Width must be between 0 and {MAX_WIDTH_OF_GAME_BOARD}");
                if (heigth <= 0 || heigth > MAX_HEIGTH_OF_GAME_BOARD) throw new Exception($"Heigth must be between 0 and {MAX_HEIGTH_OF_GAME_BOARD}");
                this.width = width;
                this.heigth = height;
                gameBoard = new Dictionary<int, Letter>(heapOfLetters.totalNounOfLetters);
            }
            
            public bool TryToPlaceLetterOnBoard(Letter letter)
            {
                if (gameBoard.ContainsKey(GetLettersHashCode(letter)))
                {
                    return false;
                }
                ////////////////

            }
        }

        public class HeapOfLetters //Куча букв из которых набирается рука
        {
            public Dictionary<char, int> letters { get; } //словарь букв
            public int totalNounOfLetters { get; }
            HeapOfLetters(Dictionary<char, int> letters)
            {
                this.letters = letters;
                totalNounOfLetters = 0;
                foreach (KeyValuePair<char, int> kvp in this.letters)
                {
                    if (!(kvp.Value >= 0) && kvp.Value <= MAX_AMOUNT_OF_EACH_LETTER_IN_HEAP)
                        throw new Exception($"Letters ammount must be between 0 and {MAX_AMOUNT_OF_EACH_LETTER_IN_HEAP}");
                    totalNounOfLetters += kvp.Value;
                }
                if (totalNounOfLetters == 0) throw new Exception("There are must be at least one letter");
            }

            public bool CheckAmIOnlyRussian()
            {
                foreach (KeyValuePair<char, int> kvp in letters)
                    if (!(kvp.Value >= 'а' && kvp.Value <= 'я' || kvp.Value == '*'))
                        return false;
                return true;
            }

        }
        public class Hand //Рука с буквами, которые будут выложены на доску
        {
            public enum LetterStatus 
            {
                InHand = 0,
                OnBoard,
                FinishedPlacing
            }
            LetterStatus[] letterStatuses;
            Letter[] letters;
        }

    }
   
    
}
