using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WzComparerR2.WzLib;

public class WzMob
{
    private Wz_Image _img;
    private Wz_Node _root;

    public WzMob()
    {

    }

    public WzMob(Wz_Image img)
    {
        _img = img;
        _root = img.Node;
    }

    public Wz_Image img { get => _img; set => _img = value; }
    public Wz_Node root { get => _root; set => _root = value; }    
}