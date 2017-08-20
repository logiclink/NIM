using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LogicLink {
    public enum Strategy {
        Misère,
        Smallest,
        Largest
    }

    /// <summary>
    /// Represents a single move within a NIM game
    /// </summary>
    public struct Move {

        /// <summary>
        /// Converts the string representation of a Move to its struct. A return value indicates whether the operation succeeded.
        /// </summary>
        /// <param name="s">A string containing a move to convert.</param>
        /// <param name="m">Parsed Move struct. This parameter is passed uninitialized; any value originally supplied in result will be overwritten.</param>
        /// <returns>true if s was converted successfully; otherwise, false.</returns>
        public static bool TryParse(string s, out Move m) {
            int i = 0, j = 0;
            bool b = true;
            string[] t = s.Split(':');

            if(t.Length != 2)                   b = false;
            else if(!int.TryParse(t[0], out i) || i < 1) b = false;
            else if(!int.TryParse(t[1], out j) || j < 1) b = false;

            m = new Move(i - 1, j);
            return b;
        }

        public readonly int Heap;
        public readonly int Count;

        public Move(int iHeap, int iCount) {
            this.Heap = iHeap;
            this.Count = iCount;
        }


        public override string ToString() {
            return $"{this.Heap + 1}:{this.Count}";
        }
    }

    public class NIM {

        #region Private variables

        private Strategy _eStrategy = Strategy.Smallest;        // Strategy used by computer moves
        private bool _bMisère = false;                          // Last object taken looses
        private int _iUpper = 0;                                // Maximum number of elements which can be removed
        private List<int> _lPlayground = new List<int>();       // Game playground with heaps and their elements 

        #endregion

        /// <summary>
        /// Returns the Xored value of all playground heaps
        /// </summary>
        /// <returns>Xored value</returns>
        public int Xor() {
            if(_lPlayground.Count == 0) throw new InvalidOperationException("Playground not initialized or end of game.");
            int x = _lPlayground[0];
            for(int i = 1; i < _lPlayground.Count; i++)
                x = x ^ _lPlayground[i];
            return x;
        }

        /// <summary>
        /// Returns the Xored value of all playground heaps subtracted by iCount in the heap iHeap
        /// </summary>
        /// <param name="m">Move with heap index and number of subtracted elements</param>
        /// <returns>Xored value</returns>
        private int Xor(Move m) {
            if(_lPlayground.Count == 0) throw new InvalidOperationException("Playground not initialized or end of game.");
            int x = m.Heap == 0 ? _lPlayground[m.Heap] - m.Count : _lPlayground[0];
            for(int i = 1; i < _lPlayground.Count; i++)
                x = x ^ (i == m.Heap ? _lPlayground[i] - m.Count : _lPlayground[i]);
            return x;
        }

        /// <summary>
        /// Returns all moves which result in Xor = 0
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Move> GetMoves() { 
            for(int i = 0; i<_lPlayground.Count; i++)
                for(int j = 1; j <= _lPlayground[i]; j++)
                    if(_bMisère) {
                        if(Xor(new Move(i, j)) != 0)
                            yield return new Move(i, j);
                    } else {
                        if(Xor(new Move(i, j)) == 0)
                            yield return new Move(i, j);
                    }
        }

        /// <summary>
        /// Gets or sets the strategy of computer moves
        /// </summary>
        public Strategy Strategy {
            get { return _eStrategy; }
            set { _eStrategy = value; }
        }

        public bool Misère {
            get { return _bMisère; }
            set { _bMisère = value; }
        }


        /// <summary>
        /// Sets or return the maximum number of elements which can be removed
        /// </summary>
        public int Upper {
            get { return _iUpper; }
            set { _iUpper = value; }
        }

        /// <summary>
        /// Playground with heaps of elements
        /// </summary>
        public List<int> Playground => _lPlayground;

        /// <summary>
        /// Calculates a new move
        /// </summary>
        /// <returns>Move</returns>
        public Move Play() {

            // Calculate optimal moves
            IEnumerable<Move> ms = _iUpper != 0
                                   ? GetMoves().Where(m => m.Count <= _iUpper)
                                   : ms = GetMoves();

            // Return optimal move
            if(ms.Count() != 0) {
                switch(_eStrategy) {
                    case Strategy.Smallest: return ms.OrderBy(m => m.Count).First();
                    case Strategy.Largest:  return ms.OrderByDescending(m => m.Count).First();
                }
            }

            // Optimal move not possible, return first possible move
            switch(_eStrategy) {
                case Strategy.Smallest:
                    int iMin = _lPlayground.Where(i => i != 0).Min();
                    return new Move(_lPlayground.IndexOf(iMin), 1);
                case Strategy.Largest:
                    int iMax = _iUpper != 0
                               ? _lPlayground.Where(i => i <= _iUpper).Max()
                               : _lPlayground.Max();
                    return new Move(_lPlayground.IndexOf(iMax), iMax);
                default:
                    for(int i = 0; i < _lPlayground.Count; i++)
                        if(_lPlayground[i] != 0)
                            return new Move(i, 1);
                    break;
            }

            throw new InvalidOperationException("No more moves possible.");
        }

        public Move Play(Random rnd) {
            List<int> lPlaygroundNotEmpty = _lPlayground.Where(i => i != 0).ToList();
            if(lPlaygroundNotEmpty.Count == 0)
                throw new InvalidOperationException("Playground not initialized or end of game.");

            int iHeap = lPlaygroundNotEmpty[(int)Math.Floor(rnd.NextDouble() * lPlaygroundNotEmpty.Count)];
            return new Move(_lPlayground.IndexOf(iHeap), (int)Math.Round(rnd.NextDouble() * (_iUpper == 0 ? iHeap : Math.Min(_iUpper, iHeap)) - 1) + 1);
        }


        public void Apply(Move m) {
            if(m.Heap < 0 || m.Heap >= _lPlayground.Count)
                throw new ArgumentOutOfRangeException(nameof(m.Heap), "Unknown heap index");
            if(_lPlayground[m.Heap] < m.Count)
                throw new ArgumentOutOfRangeException(nameof(m.Count), "Removed elements exceeded number of available elements");
            if(_iUpper != 0 && _iUpper < m.Count)
                throw new ArgumentOutOfRangeException(nameof(m.Count), "Removed elements exceeded number of allowed elements to be removed");

            _lPlayground[m.Heap] -= m.Count;
        }

        public bool EndOfGame => _lPlayground.Sum(i => i) == 0;

    }
}
