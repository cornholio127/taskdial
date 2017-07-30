namespace TaskDial
{
    public class ProcessItem
    {
        public class ProcessItemBuilder
        {
            private string _name;
            private int _handle;

            public ProcessItemBuilder Name(string name)
            {
                _name = name;
                return this;
            }

            public ProcessItemBuilder Handle(int handle)
            {
                _handle = handle;
                return this;
            }

            public ProcessItem Build()
            {
                return new ProcessItem(_name, _handle);
            }
        }

        public static ProcessItemBuilder Builder()
        {
            return new ProcessItemBuilder();
        }

        public string Name { get; private set; }
        public int Handle { get; private set; }

        public ProcessItem(string name, int handle)
        {
            Name = name;
            Handle = handle;
        }
    }
}
