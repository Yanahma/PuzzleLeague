using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuzzleLeague.Utilities.Containers
{
   // Defines a "Board" - has a set size for columns, but can add rows freely
   public class Board<T> : IEnumerable
   {
      // 
      // Private members
      //
      private List<T[]> rows;

      //
      // Public members
      //
      public int Rows
      {
         get { return rows.Count; }
      }

      public readonly int ColumnSize;

      //
      // Constructor
      //
      public Board(int columnSize)
      {
         this.ColumnSize = columnSize;
         rows = new List<T[]>();
      }

      //
      // Accessors for structure
      //

      public T[] this[int index]
      {
         get
         {
            return rows[index];
         }
      }

      public T this[int row, int column]
      {
         get
         {
            return rows[row][column];
         }
      }

      public T[] GetRow(int index)
      {
         return this[index];
      }

      public T[] GetColumn(int index)
      {
         T[] column = new T[rows.Count];
         for (var i = 0; i < rows.Count; i++)
         {
            column[i] = rows[i][index];
         }
         return column;        
      }

      //
      // Methods to modify the board's contents
      //
      
      public void AddAbove(T[] row) => rows.Insert(0, row);

      public void AddBelow(T[] row) => rows.Add(row);

      public void RemoveAbove() => rows.RemoveAt(0);

      public void RemoveBelow() => rows.RemoveAt(rows.Count - 1);

      public void RemoveAll() => rows.Clear();

      public void ReplaceRowAt(int index, T[] row) => rows[index] = row;

      public void ReplaceItemAt(int rowIndex, int columnIndex, T item) => rows[rowIndex][columnIndex] = item;

      public void SwapRow(int indexAt, int indexTo)
      {
         var swp = rows[indexTo];
         rows[indexTo] = rows[indexAt];
         rows[indexAt] = swp;
      }

      public void SwapItemOnRow(int rowIndex, int columnAt, int columnTo)
      {
         var swp = rows[rowIndex][columnTo];
         rows[rowIndex][columnTo] = rows[rowIndex][columnTo];
         rows[rowIndex][columnAt] = swp;
      }

      public void SwapItem(int rowIndex1, int columnIndex1, int rowIndex2, int columnIndex2)
      {
         var swp = rows[rowIndex2][columnIndex2];
         rows[rowIndex2][columnIndex2] = rows[rowIndex1][columnIndex1];
         rows[rowIndex1][columnIndex1] = swp;
      }

      //
      // IEnumerator implimentation
      //

      public IEnumerator GetEnumerator()
      {
         return ((IEnumerable)rows).GetEnumerator();
      }
   }
}
