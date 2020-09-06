using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnnoDesigner.Core.Models;
using AnnoDesigner.Models;

namespace AnnoDesigner.Services
{
    public class UndoRedoService
    {
        private static readonly List<LayoutObject> Empty = new List<LayoutObject>(0);
        public Stack<List<LayoutObject>> undoStates = new Stack<List<LayoutObject>>();
        public Stack<List<LayoutObject>> redoStates = new Stack<List<LayoutObject>>();

        public List<LayoutObject> CurrentState => undoStates.Peek();

        public UndoRedoService()
        {

        }

        public void UpdateState(List<LayoutObject> newState)
        {
            undoStates.Push(newState);
        }

        public List<LayoutObject> Undo()
        {
            var state = Empty;
            if (undoStates.Count != 0)
            {
                state = undoStates.Pop();
                redoStates.Push(state);
            }
            return state;
        }

        public List<LayoutObject> Redo()
        {
            var state = CurrentState;
            if (redoStates.Count != 0)
            {
                state = redoStates.Pop();
                undoStates.Push(state);
            }
            return state;
        }
    }
}
