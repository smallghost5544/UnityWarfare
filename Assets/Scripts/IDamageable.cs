
public interface IDamageable 
{
    int CurrentHp { get; set; }

    void GetHurt(int damage);
}
public enum ObjectType
{
    Unit,
    NeutralObject
}
