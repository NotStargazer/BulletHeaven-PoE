namespace Global
{
    public interface IDataBehaviour
    {
        public bool Enabled { get; }
        public void Update(float deltaTime, int behaviourId);
    }
}
