using System;
using System.Collections.Generic;
using UnityEngine;

public class GraphicsPipeline : MonoBehaviour
{
    public GameObject ScreenGO;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Model mymodel = new Model();
        mymodel.CreateUnityGameObject();


        //Vertices
        List<Vector4> verts = Homog(mymodel.vertices);
        Display("Verticies", verts);


        // First Transformation
        // Rotation by 38 degrees about (21,-3,-3).normalized
        Vector3 axis = (new Vector3(21,-3,-3)).normalized;


        //RotationMatrix
        Matrix4x4 rotationMatrix = Matrix4x4.TRS(Vector3.zero,Quaternion.AngleAxis(38, axis),Vector3.one);
        Display("RotationMatrix", rotationMatrix);


        //ImageAfterRotation
        List<Vector4> imageAfterRotation = MatrixTransform(rotationMatrix, verts);
        Display("ImageAfterRotation", imageAfterRotation);


        // 2nd Transformation Scale be (3,2,3)
        //ScaleMatrix
        Matrix4x4 scaleMatrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity,new Vector3(3, 2, 3));
        Display("ScaleMatrix", scaleMatrix);


        //ImageAfterScale
        List<Vector4> imageAftertheScale = MatrixTransform(scaleMatrix, imageAfterRotation);
        Display("imageAftertheScale", imageAftertheScale);


        //TranslationMatrix
        Matrix4x4 translationMatrix = Matrix4x4.TRS(new Vector3(3, 3, -2),Quaternion.identity,Vector3.one);
        Display("Translation matrix", translationMatrix);


        //ImageAfterTranslation
        List<Vector4> imageAfterTranslation = MatrixTransform(translationMatrix, imageAftertheScale);
        Display("imageAfterTranslation", imageAfterTranslation);


        //SingleMatrixofTransformations
        Matrix4x4 singleMoT = translationMatrix * scaleMatrix * rotationMatrix;
        Display("SingleMoT", singleMoT);


        //ImageafterTransformations
        List<Vector4> imageafterSingleMoT = MatrixTransform(singleMoT, verts);
        Display("ImageAfterMoT", imageafterSingleMoT);


        //ViewMatrix
        Vector3 eye = new Vector3(23, 0, 47);
        Vector3 target = new Vector3(-3, 3, 3);
        Vector3 up = new Vector3(-2, -3, 21).normalized;

        Matrix4x4 cameraTransform = Matrix4x4.LookAt(eye, target, up);
        Matrix4x4 viewMatrix = cameraTransform.inverse;
        Display("ViewMatrix", viewMatrix);

        List<Vector4> imageAfterView = MatrixTransform(viewMatrix, imageAfterTranslation);
        Display("ImageAfterView", imageAfterView);


        //ProjectionMatrix
        Matrix4x4 projMatrix = Matrix4x4.Perspective(60f, 16f / 9f, 0.3f, 1000f);
        Display("ProjectionMatrix", projMatrix);


        //EverythingMatrix
        Matrix4x4 EverythingMatrix = projMatrix * viewMatrix * singleMoT;
        Display("SingleMatrixEverything", EverythingMatrix);


        //FinalImage
        List<Vector4> FinalImage = MatrixTransform(projMatrix, imageAfterView);
        Display("FinalImage", FinalImage);


        //FinalImage2
        List<Vector4> FinalImage2 = MatrixTransform(EverythingMatrix, verts);
        Display("FinalImage2", FinalImage2);


        Vector4[] pts = new Vector4[]
        {
            new Vector4(-4.65f, -7.57f, -53.62f, -52.99f), new Vector4(-3.12f, -20.15f, -48.60f, -47.97f),
            new Vector4(-3.43f, -4.24f, -56.85f, -56.22f), new Vector4(-2.41f, -12.63f, -53.51f, -52.88f),
            new Vector4(-1.39f, -21.02f, -50.16f, -49.53f), new Vector4(-0.88f, -25.22f, -48.49f, -47.86f),
            new Vector4(-1.90f, -16.83f, -51.84f, -51.21f), new Vector4(2.80f, -15.25f, -58.19f, -57.56f),
            new Vector4(3.31f, -19.44f, -56.52f, -55.89f), new Vector4(4.53f, -16.12f, -59.76f, -59.12f),
            new Vector4(4.02f, -11.93f, -61.43f, -60.79f), new Vector4(1.78f, -6.86f, -61.54f, -60.90f),
            new Vector4(-3.03f, -1.82f, -49.74f, -49.11f), new Vector4(-1.50f, -14.40f, -44.73f, -44.10f),
            new Vector4(-1.80f, 1.51f, -52.98f, -52.35f), new Vector4(-1.80f, 1.51f, -52.98f, -52.35f),
            new Vector4(-1.39f, -21.02f, -50.16f, -49.53f), new Vector4(0.75f, -19.47f, -44.61f, -43.99f),
            new Vector4(-0.27f, -11.08f, -47.96f, -47.33f), new Vector4(4.42f, -9.50f, -54.32f, -53.69f),
            new Vector4(4.93f, -13.69f, -52.65f, -52.01f), new Vector4(6.16f, -10.37f, -55.88f, -55.25f),
            new Vector4(5.65f, -6.18f, -57.55f, -56.92f), new Vector4(3.40f, -1.11f, -57.66f, -57.03f)
        };

        //LineClipping Test
        foreach (Vector4 p in pts)
        {
            float xp = p.x / -p.z;   // x divided by z (minus sign because the plane is z = -1)
            float yp = p.y / -p.z;   // y divided by z
            print($"{xp:F4}, {yp:F4}");
        }

        Vector2 S = new Vector2(1.1f, 0f);
        Vector2 E = new Vector2 (-1.5f, 0.5f);

        print("LineToClip Start =" + S.ToString() + "End = " + E.ToString());
        if (LineClip(ref S, ref E))
        {
            print("Accepted Start =" + S.ToString() + "End = " + E.ToString());
        }
        else print("Rejected");




        Texture2D Screen = new Texture2D(1024, 1024);
        //Besh Test
        Vector2Int bs = new Vector2Int(512, 512);
        Vector2Int be = new Vector2Int(1023, 1023);

        print("Bresenham Start = " + bs.ToString() + " End = " + be.ToString());

        List<Vector2Int> linePoints = bresh(bs, be);
        foreach (Vector2Int point in linePoints)
        {
            Screen.SetPixel(point.x, point.y,Color.red);
        }

        ScreenGO.GetComponent<Renderer>().material.mainTexture=Screen;
        Screen.Apply();
    }



    private void Display(string name, Matrix4x4 matrix)
    {
        print($"===== {name} =====");
        for (int i = 0; i < 4; i++)
            print(matrix.GetRow(i));
    }

    private void Display(string name, List<Vector4> verts)
    {
        print($"===== {name} =====");
        foreach (Vector4 v in verts)
            print(v);
    }

    private List<Vector4> Homog(List<Vector3> vertices)
    {
        List<Vector4> result = new List<Vector4>();

        foreach (Vector3 v in vertices)
        { result.Add(new Vector4(v.x, v.y, v.z, 1f)); }

        return result;
    }

    List<Vector2Int> bresh(Vector2Int s, Vector2Int e)
    {
        List<Vector2Int> result = new List<Vector2Int>();

        int dx = e.x - s.x;

        if (dx < 0)
        {
            return bresh(e, s);
        }

        int dy = e.y - s.y;

        if(dy < 0)
        {
            return NegY(bresh(NegY(s), NegY(e)));
        }

        if (dy > dx)
        {
            return SwapXY(bresh(SwapXY(s), SwapXY(e)));
        }

        int negative = 2 * (dy - dx); //orange
        int positive = 2 * dy;        //yellow
        int p = 2 * dy - dx;          //green

        int x = s.x;
        int y = s.y;

        result.Add(new Vector2Int(x, y));

        while (x < e.x)
        {
            x++;

            if (p < 0)
            {
                p += positive;
            }
            else
            {
                y++;
                p += negative;
            }
            result.Add(new Vector2Int(x, y));
        }

        return result;
    }

    private List<Vector2Int> SwapXY(List<Vector2Int> vector2Ints)
    {
        List<Vector2Int> result = new List<Vector2Int>();
        foreach (Vector2Int v in vector2Ints)
            result.Add(SwapXY(v));

        return result;
    }

    private Vector2Int SwapXY(Vector2Int s)
    {
        return new Vector2Int(s.y, s.x);
    }

    private List<Vector2Int> NegY(List<Vector2Int> vector2Ints)
    {
        List<Vector2Int> result = new List<Vector2Int>();
        foreach (Vector2Int v in vector2Ints)
            result.Add(NegY(v));
            

            return result;
    }

    private Vector2Int NegY(Vector2Int s)
    {
        return new Vector2Int(s.x, -s.y);
    }

    bool LineClip(ref Vector2 S, ref Vector2 E)
    {
        Outcode StartO = new Outcode(S);
        Outcode EndE = new Outcode(E);
        Outcode Inviewport = new Outcode();

        if ((StartO + EndE) == Inviewport)
            return true;

        if ((StartO * EndE) != Inviewport)
            return false;


        if (StartO == Inviewport)
            return LineClip(ref E, ref S);

        if (StartO.up)
        {
            if (E.x == S.x)
            {
                S = new Vector2(E.x, 1); 
                return LineClip(ref S, ref E);
            }
            if (E.y == S.y)
            {
                return false;
            }

            float m = (E.y - S.y) / (E.x - S.x);
            S = new Vector2(S.x + (1 / m) * (1 - S.y), 1);
            return LineClip(ref S, ref E);
        }
        if (StartO.down)
        {
            if (E.x == S.x)
            {
                S = new Vector2(E.x, -1);
                return LineClip(ref S, ref E);
            }
            if (E.y == S.y)
            {
                return false;
            }

            float m = (E.y - S.y) / (E.x - S.x);
            S = new Vector2(S.x + (1 / m) * (-1 - S.y), -1);
            return LineClip(ref S, ref E);
        }

        if (StartO.left)
        {
            if (E.x == S.x)
            {
                return false;
            }

            float m = (E.y - S.y) / (E.x - S.x);
            S = new Vector2(-1, S.y + m * (-1 - S.x));
            return LineClip(ref S, ref E);
        }

        if (StartO.right)
        {
            if (E.x == S.x)
            {
                return false;
            }

            float m = (E.y - S.y) / (E.x - S.x);
            S = new Vector2(1, S.y + m * (1 - S.x));
            return LineClip(ref S, ref E);
        }

        return false;
    }




    private List<Vector4> MatrixTransform(Matrix4x4 matrix, List<Vector4> verts)
    {
        List<Vector4> hold = new List<Vector4>();
        foreach (Vector4 v in verts)
            hold.Add(matrix * v);

        return hold;
    }

    private void Display(Matrix4x4 matrix)
    {
        for (int i = 0; i < 4; i++)
            print(matrix.GetRow(i));
    }

    void Display(List<Vector4> verts)
    {
        foreach (Vector4 v in verts) { print(v); }
    }

    Vector4 Proj1(Vector4 v)
    {
        return new Vector4(v.x / v.w,v.y/v.w,v.z,v.w);
    }

    Vector2Int Pixelize(Vector4 v)
    {
        return new Vector2Int(Mathf.RoundToInt((v.x + 1)* 1023/2),Mathf.RoundToInt((v.y + 1) * 1023 / 2));
    }




    // Update is called once per frame
    void Update()
    {
        Matrix4x4 World = Matrix4x4.TRS(new Vector3(0, 0, 10), Quaternion.identity, Vector3.one);
        Matrix4x4 View = Matrix4x4.LookAt(Vector3.zero, new Vector3(0, 0, 10), Vector3.up);
        Matrix4x4 Proj = Matrix4x4.Perspective(90, 1, 1, 1000);
        Matrix4x4 Super = Proj * View * World;

        Model mymodel = new Model();
        List<Vector4> verts = Homog(mymodel.vertices);
        List<Vector4> Transformed = MatrixTransform(Super, verts);

        List<Vector2Int> Pixels = new List<Vector2Int>();
        foreach (Vector4 v in Transformed)
        {
            Pixels.Add(Pixelize(Proj1(v)));

        }



        Vector3Int Face = mymodel.faces[0];
        Vector2Int v1 = Pixels[Face.x];
        Vector2Int v2 = Pixels[Face.y];
        Vector2Int v3 = Pixels[Face.z];

        Texture2D Screen = new Texture2D(1024, 1024);


        //DrawLine(Screen, v1, v2);
        //DrawLine(Screen, v2, v3);
        //DrawLine(Screen, v3, v1);

        foreach (Vector3Int f in mymodel.faces)
        {
            DrawLine(Screen, Pixels[f.x], Pixels[f.y]);
            DrawLine(Screen, Pixels[f.y], Pixels[f.z]);
            DrawLine(Screen, Pixels[f.z], Pixels[f.x]);
        }

        ScreenGO.GetComponent<Renderer>().material.mainTexture = Screen;
        Screen.Apply();
    }

    private void DrawLine(Texture2D Screen, Vector2Int bs, Vector2Int be)
    {
        List<Vector2Int> linePoints = bresh(bs, be);
        foreach (Vector2Int point in linePoints)
        {
            Screen.SetPixel(point.x, point.y, Color.red);
        }
    }
}
