namespace TheoryOfAutomatons.Utils.Containers
{
    internal class DescribedObject<T>
    {
        public T Value { get; set; }
        public string Description { get; set; }

        public DescribedObject() { }

        public DescribedObject(T value, string description)
        {
            Value = value;
            Description = description;
        }

        public override string ToString()
        {
            if (Description == null || Description == string.Empty)
                return $"{Value}";
            return $"{Value} - {Description}";
        }
    }
}
