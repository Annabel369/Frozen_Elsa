using System.Diagnostics;


namespace Frozen_Elsa;

public class Globals
{
    //private const int PLAYER_ONFIRE = (1 << 24);

   // private bool[] bUserHasFrozen = new bool[33];
    
    public static string SiteImage = "https://c.tenor.com/nEu74vu_sT4AAAAC/tenor.gif";

  
    //private const string IceModel = "models/weapons/eminem/ice_cube/ice_cube.mdl";//    private const string IceCube3d = "materials/weapons/eminem/ice_cube/ice_cube.vmt";

    public static char Default = '\x01';
    public static char White = '\x01';
    public static char Darkred = '\x02';
    public static char Green = '\x04';
    public static char LightYellow = '\x03';
    public static char LightBlue = '\x03';
    public static char Olive = '\x05';
    public static char Lime = '\x06';
    public static char Red = '\x07';
    public static char Purple = '\x03';
    public static char Grey = '\x08';
    public static char Yellow = '\x09';
    public static char Gold = '\x10';
    public static char Silver = '\x0A';
    public static char Blue = '\x0B';
    public static char DarkBlue = '\x0C';
    public static char BlueGrey = '\x0D';
    public static char Magenta = '\x0E';
    public static char LightRed = '\x0F';

    public bool Enable { get; set; } = false;


    public static int Takefreezetime;
    public static Stopwatch Timers = new Stopwatch();
    public static bool First_Blood = false;

    


}