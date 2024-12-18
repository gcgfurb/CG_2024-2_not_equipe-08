//https://github.com/mono/opentk/blob/main/Source/Examples/Shapes/Old/Cube.cs

#define CG_Debug
using CG_Biblioteca;
using OpenTK.Graphics.OpenGL4;
using System.Collections.Generic;
using OpenTK.Mathematics;
using System.Drawing;

namespace gcgcg
{
  internal class Cubo : Objeto
  {
    Ponto4D[] vertices;

    private readonly float[] _frontFaceVertices =
    [
      // Positions          Normals              Texture coords
      -0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  0.0f, 0.0f,
       0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  1.0f, 0.0f,
       0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  1.0f, 1.0f,
       0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  1.0f, 1.0f,
      -0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  0.0f, 1.0f,
      -0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  0.0f, 0.0f
    ];
    
    private readonly float[] _backFaceVertices =
    [
      // Positions          Normals              Texture coords
      -0.5f, -0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  0.0f, 0.0f,
       0.5f, -0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  1.0f, 0.0f,
       0.5f,  0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  1.0f, 1.0f,
       0.5f,  0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  1.0f, 1.0f,
      -0.5f,  0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  0.0f, 1.0f,
      -0.5f, -0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  0.0f, 0.0f
    ];

    private readonly float[] _topFaceVertices =
    [
      // Positions          Normals              Texture coords
      -0.5f,  0.5f,  0.5f, -1.0f,  0.0f,  0.0f,  1.0f, 0.0f,
      -0.5f,  0.5f, -0.5f, -1.0f,  0.0f,  0.0f,  1.0f, 1.0f,
      -0.5f, -0.5f, -0.5f, -1.0f,  0.0f,  0.0f,  0.0f, 1.0f,
      -0.5f, -0.5f, -0.5f, -1.0f,  0.0f,  0.0f,  0.0f, 1.0f,
      -0.5f, -0.5f,  0.5f, -1.0f,  0.0f,  0.0f,  0.0f, 0.0f,
      -0.5f,  0.5f,  0.5f, -1.0f,  0.0f,  0.0f,  1.0f, 0.0f
    ];

    private readonly float[] _bottomFaceVertices =
    [
      // Positions          Normals              Texture coords
      0.5f,  0.5f,  0.5f,  1.0f,  0.0f,  0.0f,  1.0f, 0.0f,
      0.5f,  0.5f, -0.5f,  1.0f,  0.0f,  0.0f,  1.0f, 1.0f,
      0.5f, -0.5f, -0.5f,  1.0f,  0.0f,  0.0f,  0.0f, 1.0f,
      0.5f, -0.5f, -0.5f,  1.0f,  0.0f,  0.0f,  0.0f, 1.0f,
      0.5f, -0.5f,  0.5f,  1.0f,  0.0f,  0.0f,  0.0f, 0.0f,
      0.5f,  0.5f,  0.5f,  1.0f,  0.0f,  0.0f,  1.0f, 0.0f
    ];

    public readonly float[] _rightFaceVertices =
    [
      // Positions          Normals              Texture coords
      -0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,  0.0f, 1.0f,
       0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,  1.0f, 1.0f,
       0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,  1.0f, 0.0f,
       0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,  1.0f, 0.0f,
      -0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,  0.0f, 0.0f,
      -0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,  0.0f, 1.0f
    ];

    public readonly float[] _leftFaceVertices =
    [
      // Positions          Normals              Texture coords
      -0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,  0.0f, 1.0f,
       0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,  1.0f, 1.0f,
       0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,  1.0f, 0.0f,
       0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,  1.0f, 0.0f,
      -0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,  0.0f, 0.0f,
      -0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,  0.0f, 1.0f
    ];

    private readonly uint[] _frontFaceIndices =
    [
      1, 2, 3,
      0, 1, 3,
    ];

    private readonly uint[] _backFaceIndices =
    [
      1, 2, 3,
      0, 1, 3
    ];

    private readonly uint[] _topFaceIndices =
    [
      3, 0, 1,
      0, 2, 1
    ];

    private readonly uint[] _bottomFaceIndices =
    [
      3, 0, 1,
      0, 2, 1
    ];

    private readonly uint[] _rightFaceIndices =
    [
      1, 0, 2,
      3, 0, 1
    ];

    private readonly uint[] _leftFaceIndices =
    [
      1, 0, 2,
      3, 0, 1
    ];

    public Cubo(Objeto _paiRef, ref char _rotulo) : base(_paiRef, ref _rotulo)
    {
      PrimitivaTipo = PrimitiveType.TriangleFan;
      PrimitivaTamanho = 10;

      vertices = new Ponto4D[]
      {
        new Ponto4D(-1.0f, -1.0f,  1.0f),
        new Ponto4D( 1.0f, -1.0f,  1.0f),
        new Ponto4D( 1.0f,  1.0f,  1.0f),
        new Ponto4D(-1.0f,  1.0f,  1.0f),
        new Ponto4D(-1.0f, -1.0f, -1.0f),
        new Ponto4D( 1.0f, -1.0f, -1.0f),
        new Ponto4D( 1.0f,  1.0f, -1.0f),
        new Ponto4D(-1.0f,  1.0f, -1.0f)
      };

      // // 0, 1, 2, 3 Face da frente
      base.PontosAdicionar(vertices[0]);
      base.PontosAdicionar(vertices[1]);
      base.PontosAdicionar(vertices[2]);
      base.PontosAdicionar(vertices[3]);

      // // 3, 2, 6, 7 Face de cima
      base.PontosAdicionar(vertices[3]);
      base.PontosAdicionar(vertices[2]);
      base.PontosAdicionar(vertices[6]);
      base.PontosAdicionar(vertices[7]);
      
      // // 4, 7, 6, 5 Face do fundo
      base.PontosAdicionar(vertices[4]);
      base.PontosAdicionar(vertices[7]);
      base.PontosAdicionar(vertices[6]);
      base.PontosAdicionar(vertices[5]);
      
      // // 0, 3, 7, 4 Face direita
      base.PontosAdicionar(vertices[0]);
      base.PontosAdicionar(vertices[3]);
      base.PontosAdicionar(vertices[7]);
      base.PontosAdicionar(vertices[4]);

      // // 0, 4, 5, 1 Face de baixo
      base.PontosAdicionar(vertices[0]);
      base.PontosAdicionar(vertices[4]);
      base.PontosAdicionar(vertices[5]);
      base.PontosAdicionar(vertices[1]);

      // // 1, 5, 6, 2 Face direita
      base.PontosAdicionar(vertices[1]);
      base.PontosAdicionar(vertices[5]);
      base.PontosAdicionar(vertices[6]);
      base.PontosAdicionar(vertices[2]);

      Atualizar();
    }

    private void Atualizar()
    {

      base.ObjetoAtualizar();
    }

     public List<float[]> GetFaceVertices()
    {
      List<float[]> faceVertices =
      [
        _frontFaceVertices,
        _backFaceVertices,
        _topFaceVertices,
        _bottomFaceVertices,
        _rightFaceVertices,
        _leftFaceVertices
      ];
      return faceVertices;
    }

    public List<uint[]> GetFaceIndices()
    {
      List<uint[]> faceIndices =
      [
        _frontFaceIndices,
        _backFaceIndices,
        _topFaceIndices,
        _bottomFaceIndices,
        _rightFaceIndices,
        _leftFaceIndices
      ];
      return faceIndices;
    }

#if CG_Debug
    public override string ToString()
    {
      string retorno;
      retorno = "__ Objeto Cubo _ Tipo: " + PrimitivaTipo + " _ Tamanho: " + PrimitivaTamanho + "\n";
      retorno += base.ImprimeToString();
      return (retorno);
    }
#endif

  }
}
