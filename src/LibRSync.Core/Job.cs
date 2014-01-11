using System.Reflection.Emit;

namespace LibRSync.Core
{
    internal abstract class Job
    {
        protected delegate StateFunc StateFunc();

        private string name;
        private StateFunc stateFunc;

        protected Job(string name)
        {
            this.name = name;
            stateFunc = InitialState;
        }

        protected abstract StateFunc InitialState();

        protected StateFunc Completed()
        {
            stateFunc = Completed;
            return Completed;
        }

        public void Run()
        {
            while (true)
            {
                stateFunc = stateFunc();
                if (stateFunc == Completed)
                    break;
            }
        }

        public override string ToString()
        {
            return string.Format("{0}:{1}", name, stateFunc.Method.Name);
        }
    }
}
