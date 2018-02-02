using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcanistsRevamped
{
    private void DestroyTerrain(Color[,] tex, Matrix mat)
    {
        int width = tex.GetLength(0);
        int height = tex.GetLength(1);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (tex[x, y].R > 10)
                {
                    Vector2 imagePos = new Vector2(x, y);
                    Vector2 screenPos = Vector2.Transform(imagePos, mat);

                    int screenX = (int)screenPos.X;
                    int screenY = (int)screenPos.Y;
                }
            }
        }
    }
}
