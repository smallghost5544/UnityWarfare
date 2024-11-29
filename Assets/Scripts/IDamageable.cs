
public interface IDamageable 
{
    int CurrentHp { get; set; }
    void GetHurt(int damage);
    ObjectType GetObjectType(); 
}
public enum ObjectType
{
    Unit,
    Building,
    Mine,
    Chest,
    NeutralObject,
}
