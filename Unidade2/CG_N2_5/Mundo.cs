/*
 As constantes dos pré-processors estão nos arquivos ".csproj"
 desse projeto e da CG_Biblioteca.
*/

using CG_Biblioteca;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Desktop;
using System;
using System.Collections.Generic;
using OpenTK.Mathematics;

namespace gcgcg
{
  public class Mundo : GameWindow
  {
    private static Objeto mundo = null;

    private char rotuloAtual = '?';
    private Dictionary<char, Objeto> grafoLista = [];
    private Objeto objetoSelecionado = null;


#if CG_Gizmo
    private readonly float[] _sruEixos =
    [
       0.0f,  0.0f,  0.0f, /* X- */      0.5f,  0.0f,  0.0f, /* X+ */
       0.0f,  0.0f,  0.0f, /* Y- */      0.0f,  0.5f,  0.0f, /* Y+ */
       0.0f,  0.0f,  0.0f, /* Z- */      0.0f,  0.0f,  0.5f  /* Z+ */
    ];
    private int _vertexBufferObject_sruEixos;
    private int _vertexArrayObject_sruEixos;
#endif

    private Shader _shaderVermelha;
    private Shader _shaderVerde;
    private Shader _shaderAzul;
    private Shader _shaderCiano;
    private Circulo circuloMenor = null;
    private Ponto ponto = null;
    private Retangulo quadrado = null;
    private double joyStickCenterX = 0.3;
    private double joyStickCenterY = 0.3;
    private Ponto4D joyStickCenterInit = new Ponto4D(0.3, 0.3);         


    public Mundo(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
      : base(gameWindowSettings, nativeWindowSettings)
    {
      mundo ??= new Objeto(null, ref rotuloAtual); //padrão Singleton
    }

    protected override void OnLoad()
    {
      base.OnLoad();

      Utilitario.Diretivas();
#if CG_DEBUG      
      Console.WriteLine("Tamanho interno da janela de desenho: " + ClientSize.X + "x" + ClientSize.Y);
#endif

      GL.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);

      #region Cores
      _shaderVermelha = new Shader("Shaders/shader.vert", "Shaders/shaderVermelha.frag");
      _shaderVerde = new Shader("Shaders/shader.vert", "Shaders/shaderVerde.frag");
      _shaderAzul = new Shader("Shaders/shader.vert", "Shaders/shaderAzul.frag");
      _shaderCiano = new Shader("Shaders/shader.vert", "Shaders/shaderCiano.frag");
      #endregion

#if CG_Gizmo
      #region Eixos: SRU  
      _vertexBufferObject_sruEixos = GL.GenBuffer();
      GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject_sruEixos);
      GL.BufferData(BufferTarget.ArrayBuffer, _sruEixos.Length * sizeof(float), _sruEixos, BufferUsageHint.StaticDraw);
      _vertexArrayObject_sruEixos = GL.GenVertexArray();
      GL.BindVertexArray(_vertexArrayObject_sruEixos);
      GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
      GL.EnableVertexAttribArray(0);
      #endregion
      #region Objeto: BBox dos círculos
      double raio = 0.3;
      objetoSelecionado = new Circulo(mundo, ref rotuloAtual, raio, joyStickCenterInit)
      {
            PrimitivaTipo = PrimitiveType.LineLoop
      };
      var pontoInf = Matematica.GerarPtosCirculo(225, 0.3);
      var pontoSup = Matematica.GerarPtosCirculo(45, 0.3);
      quadrado = new Retangulo(objetoSelecionado, ref rotuloAtual, 
            new Ponto4D(pontoInf.X + joyStickCenterInit.X, pontoInf.Y + joyStickCenterInit.Y),
            new Ponto4D(pontoSup.X + joyStickCenterInit.X, pontoSup.Y + joyStickCenterInit.Y))
      {
            PrimitivaTipo = PrimitiveType.LineLoop
      };
      circuloMenor = new Circulo(objetoSelecionado, ref rotuloAtual, 0.1, joyStickCenterInit)
      {
            PrimitivaTipo = PrimitiveType.LineLoop
      };

      ponto = new Ponto(objetoSelecionado, ref rotuloAtual, joyStickCenterInit)
      {
            PrimitivaTamanho = 10
      };

      #endregion
#endif

    }
    public void Atualizar(Ponto4D ptoDeslocamento) 
    {
      if (joyStickCenterX == ptoDeslocamento.X && joyStickCenterY == ptoDeslocamento.Y) return;
      joyStickCenterX = ptoDeslocamento.X;
      joyStickCenterY = ptoDeslocamento.Y;
      circuloMenor.Atualizar(ptoDeslocamento);
      ponto.PontosAlterar(ptoDeslocamento, 0);

    }

    protected override void OnRenderFrame(FrameEventArgs e)
    {
      base.OnRenderFrame(e);

      GL.Clear(ClearBufferMask.ColorBufferBit);

      mundo.Desenhar(new Transformacao4D(), objetoSelecionado);

#if CG_Gizmo
      Gizmo_Sru3D();
#endif
      SwapBuffers();
    }

    protected override void OnUpdateFrame(FrameEventArgs e)
    {
      base.OnUpdateFrame(e);

      #region Teclado
      var input = KeyboardState;
      var newPto = new Ponto4D(joyStickCenterX, joyStickCenterY);

      if (input.IsKeyPressed(Keys.Escape))
      {
            Close();
            return;
      }
      
      if (input.IsKeyPressed(Keys.D))
      {
            newPto.X += 0.01;
      }
      if (input.IsKeyPressed(Keys.E))
      {
            newPto.X -= 0.01;
      }
      if (input.IsKeyPressed(Keys.C))
      {
            newPto.Y += 0.01;
      }
      if (input.IsKeyPressed(Keys.B))
      {
            newPto.Y -= 0.01;
      }              

      if (quadrado.Bbox().Dentro(newPto))
      {
            quadrado.PrimitivaTipo = PrimitiveType.LineLoop;
            Atualizar(newPto); 
      }
      else if(Matematica.DistanciaQuadrado(joyStickCenterInit, newPto) < Math.Pow(0.3, 2)) Atualizar(newPto);
      #endregion
    }

    protected override void OnResize(ResizeEventArgs e)
    {
      base.OnResize(e);

#if CG_DEBUG      
      Console.WriteLine("Tamanho interno da janela de desenho: " + ClientSize.X + "x" + ClientSize.Y);
#endif
      GL.Viewport(0, 0, ClientSize.X, ClientSize.Y);
    }

    protected override void OnUnload()
    {
      mundo.OnUnload();

      GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
      GL.BindVertexArray(0);
      GL.UseProgram(0);

#if CG_Gizmo
      GL.DeleteBuffer(_vertexBufferObject_sruEixos);
      GL.DeleteVertexArray(_vertexArrayObject_sruEixos);
#endif

      GL.DeleteProgram(_shaderVermelha.Handle);
      GL.DeleteProgram(_shaderVerde.Handle);
      GL.DeleteProgram(_shaderAzul.Handle);
      GL.DeleteProgram(_shaderCiano.Handle);

      base.OnUnload();
    }

    private void Gizmo_Sru3D()
    {
#if CG_Gizmo
#if CG_OpenGL
      var transform = Matrix4.Identity;
      GL.BindVertexArray(_vertexArrayObject_sruEixos);
      // EixoX
      _shaderVermelha.SetMatrix4("transform", transform);
      _shaderVermelha.Use();
      GL.DrawArrays(PrimitiveType.Lines, 0, 2);
      // EixoY
      _shaderVerde.SetMatrix4("transform", transform);
      _shaderVerde.Use();
      GL.DrawArrays(PrimitiveType.Lines, 2, 2);
      // EixoZ
      _shaderAzul.SetMatrix4("transform", transform);
      _shaderAzul.Use();
      GL.DrawArrays(PrimitiveType.Lines, 4, 2);
#endif
#endif
    }

  }
}