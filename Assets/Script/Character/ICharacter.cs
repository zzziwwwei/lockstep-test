

public interface IAction
{
    void Right();
    void Left();
    void Jump();
    void Crouch();
}

public interface IDamagable
{
    void Damage(int damage,int stiff,string message);   
}

