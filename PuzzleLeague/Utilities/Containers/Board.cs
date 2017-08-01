using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuzzleLeague.Utilities.Containers
{
   // Defines a "Board" - has a set size for columns, but can add rows freely
   public class Board<T>
      where T : IEquatable<T>
   {
      // Defines a cell in the board
      internal struct Cell
      {
         private T item; // The actual item in the cell
         private bool empty; // Is this cell "empty" (used for compressing)
         private bool dirty; // Is this cell clean/dirty

         // Accessor for item
         public T Item
         {
            get { return item; }
            set { item = value; }
         }

         // Accessor for Empty
         public bool Empty
         {
            get { return empty; }
            set { empty = value; }
         }

         public bool Dirty
         {
            get { return dirty; }
            set { dirty = value; }
         }

         // Constructor
         internal Cell(T item, bool empty = false, bool dirty = true)
         {
            this.item = item;
            this.empty = empty;
            this.dirty = dirty;
         }

         // Converts an array of cells into an array of item T
         static internal T[] ToItems(Cell[] cells)
         {
            T[] rtnArray = new T[cells.Length];
            for (var i = 0; i < rtnArray.Length; i++)
               rtnArray[i] = cells[i].item;
            return rtnArray;
         }

         // Converts an array of item T into cells
         static internal Cell[] ToCells(T[] items)
         {
            Cell[] rtnArray = new Cell[items.Length];
            for (var i = 0; i < rtnArray.Length; i++)
               rtnArray[i] = new Cell(items[i]);
            return rtnArray;
         }
      }

      // 
      // Private members
      //
      private List<Cell[]> board;
      private readonly int columnSize;

      //
      // Public members
      //
      public int Rows
      {
         get { return board.Count; }
      }

      public int Columns
      {
         get { return columnSize; }
      }

      //
      // Constructor
      //
      public Board(int columnSize)
      {
         this.columnSize = columnSize;
         board = new List<Cell[]>();
      }

      //
      // Accessors for structure
      //

      public T[] this[int index]
      {
         get { return Cell.ToItems(board[index]); }
      }

      public T this[int row, int column]
      {
         get { return board[row][column].Item; }
      }

      public T[] GetColumn(int index)
      {
         T[] column = new T[board.Count];
         for (var i = 0; i < board.Count; i++)
         {
            column[i] = board[i][index].Item;
         }
         return column;
      }

      public T GetItem(int row, int column)
      {
         return this[row, column];
      }

      public T[] GetRow(int index)
      {
         return this[index];
      }
      //
      // Dirty methods
      //

      public bool IsRowDirty(int index)
      {
         for (var i = 0; i < Columns; i++)
         {
            if (board[index][i].Dirty)
               return true;
         }
         return false;
      }

      public bool IsColumnDirty(int index)
      {
         for (var i = 0; i < Rows; i++)
         {
            if (board[i][index].Dirty)
               return true;
         }
         return false;
      }

      public void FlagRowDirty(int index)
      {
         for (var i = 0; i < Columns; i++)
            board[index][i].Dirty = true;
      }

      public void FlagRowsDirty(int[] indexes)
      {
         foreach (int i in indexes)
            FlagRowDirty(i);
      }

      public void FlagColumnDirty(int index)
      {
         for (var i = 0; i < Rows; i++)
            board[i][index].Dirty = true;
      }

      public void FlagColumnsDirty(int[] indexes)
      {
         foreach (int i in indexes)
            FlagColumnDirty(i);
      }

      public void FlagAllDirty()
      {
         for (var i = 0; i < Rows; i++)
            FlagRowDirty(i);
      }

      public void FlagRowClean(int index)
      {
         for (var i = 0; i < Columns; i++)
            board[index][i].Dirty = false;
      }

      public void FlagRowsClean(int[] indexes)
      {
         foreach (int i in indexes)
            FlagRowClean(i);
      }

      public void FlagColumnClean(int index)
      {
         for (var i = 0; i < Rows; i++)
            board[i][index].Dirty = false;
      }

      public void FlagColumnsClean(int[] indexes)
      {
         foreach (int i in indexes)
            FlagColumnClean(i);
      }

      public void FlagAllClean()
      {
         for (var i = 0; i < Rows; i++)
            FlagRowClean(i);
      }

      //
      // Find methods
      //

      public int IndexOf(T[] row)
      {
         for (var i = 0; i < Rows; i++)
         {
            bool matches = true;
            for (var j = 0; j < row.Length && matches; j++)
            {
               if (!(row[j].Equals(board[i][j].Item)))
                  matches = false;
            }
            if (matches)
               return i;
         }
         return -1; // Fail case, return -1
      }

      //
      // Methods to modify the board's contents
      //

      public void AddAbove(T[] row) => board.Insert(0, Cell.ToCells(row));

      public void AddBelow(T[] row) => board.Add(Cell.ToCells(row));

      public void Remove(T[] row) => board.Remove(Cell.ToCells(row));

      // Doesn't actually do anything other than mark the cell as "empty" 
      public void RemoveItem(int rowIndex, int columnIndex)
      {
         board[rowIndex][columnIndex].Empty = true;
         // Flag row & column as dirty
         FlagRowDirty(rowIndex);
         FlagColumnDirty(columnIndex);
      }

      public void RemoveAbove() => board.RemoveAt(0);

      public void RemoveBelow() => board.RemoveAt(board.Count - 1);

      public void RemoveAll() => board.Clear();

      public void ReplaceRowAt(int index, T[] row) => board[index] = Cell.ToCells(row);

      public void ReplaceItemAt(int rowIndex, int columnIndex, T item)
      {
         Cell thisCell = board[rowIndex][columnIndex];
         thisCell.Item = item; // Place item at position
         thisCell.Empty = false; // Mark as not empty
         thisCell.Dirty = true; // Mark as dirty
      }

      public void SwapRow(int indexAt, int indexTo)
      {
         var swp = board[indexTo];
         board[indexTo] = board[indexAt];
         board[indexAt] = swp;
         FlagRowsDirty(new int[] { indexAt, indexTo });
      }

      public void SwapItemOnRow(int rowIndex, int columnAt, int columnTo)
      {
         var swp = board[rowIndex][columnAt];
         board[rowIndex][columnAt] = board[rowIndex][columnTo];
         board[rowIndex][columnTo] = swp;
         FlagRowDirty(rowIndex);
         FlagColumnsDirty(new int[] { columnAt, columnTo });
      }

      public void SwapItemOnColumn(int columnIndex, int rowAt, int rowTo)
      {
         var swp = board[rowAt][columnIndex];
         board[rowAt][columnIndex] = board[rowTo][columnIndex];
         board[rowTo][columnIndex] = swp;
         FlagColumnDirty(columnIndex);
         FlagRowsDirty(new int[] { rowAt, rowTo });
      }

      public void SwapItem(int rowIndex1, int columnIndex1, int rowIndex2, int columnIndex2)
      {
         var swp = board[rowIndex2][columnIndex2];
         board[rowIndex2][columnIndex2] = board[rowIndex1][columnIndex1];
         board[rowIndex1][columnIndex1] = swp;
         FlagAllDirty();
      }

      //
      // More "bespoke" code for gameboard stuff
      //

      // Remove empty rows from the top
      public void RemoveEmptyRows()
      {
         for (var i = 0; i < Rows; i++)
         {
            bool isEmpty = true;
            for (var j = 0; j < Columns && isEmpty; j++)
            {
               if (!(board[i][j].Empty))
                  isEmpty = false;
            }
            if (isEmpty)
               RemoveAbove();
            else
               return;
         }
      }

      // Move items in all columns downward if there are spaces
      public void CompressBoardDownwards()
      {
         for (var i = 0; i < Columns; i++)
         {
            CompressColumnDownwards(i);
         }
      }

      // Moves items downward is there are empty spaces
      public void CompressColumnDownwards(int columnIndex)
      {
         // Used to queue up the empty positions in the column
         Queue<int> dropPositions = new Queue<int>();

         // Loop through each row on the column
         for (var j = 0; j < Rows; j++)
         {
            if (board[j][columnIndex].Empty)
            {
               dropPositions.Enqueue(j); // Queue up an empty space
            }
            else
            {
               if (dropPositions.Count > 0) // If there are any empty spaces below us
               {
                  SwapItemOnColumn(columnIndex, dropPositions.Dequeue(), j);
                  dropPositions.Enqueue(j);
               }
            }
         }
      }
   }
}
