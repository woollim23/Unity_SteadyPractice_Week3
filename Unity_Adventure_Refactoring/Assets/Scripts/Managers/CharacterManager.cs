public class CharacterManager : Singletone<CharacterManager>
{
    public Player _player;
    public Player Player
    {
        get { return _player; }
        set { _player = value; }
    }

}
