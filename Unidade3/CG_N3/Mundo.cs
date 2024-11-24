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
using Microsoft.VisualBasic;

namespace gcgcg
{
  public class Mundo : GameWindow
  {
    private static readonly string SHADER_VERT = "Shaders/shader.vert";
    private static Objeto mundo = null;

    private char rotuloAtual = '?';
    private Dictionary<char, Objeto> grafoLista = [];

    private Objeto objetoSelecionado = null;
    private Poligono unfinishedPolygon = null;


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
    private Shader _shaderAmarela;

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

      GL.ClearColor(0.3f, 0.1f, 0.3f, 1.0f);

      #region Cores
      _shaderVermelha = new Shader(SHADER_VERT, "Shaders/shaderVermelha.frag");
      _shaderVerde = new Shader(SHADER_VERT, "Shaders/shaderVerde.frag");
      _shaderAzul = new Shader(SHADER_VERT, "Shaders/shaderAzul.frag");
      _shaderAmarela = new Shader(SHADER_VERT, "Shaders/shaderAmarela.frag");
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

      

#endif

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

    protected Ponto4D getMousePos()
    {
      Ponto4D mousePoint = new Ponto4D(MousePosition.X, MousePosition.Y);
      int windowHeight = ClientSize.Y;
      int windowWidth = ClientSize.X;
      Ponto4D sruPoint = Utilitario.NDC_TelaSRU(windowWidth, windowHeight, mousePoint);
      if(objetoSelecionado != null) {
            return objetoSelecionado.MatrizGlobalInversa(sruPoint);
      }
      return sruPoint;
    }

    protected int getClosestVertice(Ponto4D mousePos)
    {
      int closestVertice = 0;
      double minDistance = int.MaxValue;
      for (int i = 0; i < objetoSelecionado.PontosListaTamanho; i++)
      {
            double distance = Matematica.DistanciaQuadrado(mousePos, objetoSelecionado.PontosId(i));
            if (distance < minDistance)
            {
                  minDistance = distance;
                  closestVertice = i;
            }
      }
      return closestVertice;
    }

    protected override void OnUpdateFrame(FrameEventArgs e)
    {
      base.OnUpdateFrame(e);

      #region Teclado
      var estadoTeclado = KeyboardState;
      if (estadoTeclado.IsKeyPressed(Keys.Enter))
      {
            if (unfinishedPolygon != null)
            {
                  objetoSelecionado = unfinishedPolygon;
                  unfinishedPolygon = null;
            }
      }
      if (estadoTeclado.IsKeyPressed(Keys.D))
      {
            Poligono polygon = (Poligono)(unfinishedPolygon ?? objetoSelecionado);
            if (polygon != null)
            {
                  grafoLista.Remove(polygon.Rotulo);
                  polygon.ObjetoRemover();
                  unfinishedPolygon = null;
                  objetoSelecionado = null;
                  Grafocena.GrafocenaAtualizar(mundo, grafoLista);
            }
      }
      if (estadoTeclado.IsKeyDown(Keys.V))
      {
            if (objetoSelecionado != null)
            {
                  Ponto4D mousePos = getMousePos();
                  int closestVertice = getClosestVertice(mousePos);
                  objetoSelecionado.PontosAlterar(mousePos, closestVertice);
            }
      }
      if (estadoTeclado.IsKeyPressed(Keys.E))
      {
            if (objetoSelecionado != null)
            {
                  Ponto4D mousePos = getMousePos();
                  int closestVertice = getClosestVertice(mousePos);
                  objetoSelecionado.PontosRemover(closestVertice);
            }
      }
      if (estadoTeclado.IsKeyPressed(Keys.P))
      {
            if (objetoSelecionado != null)
            {
                 if (objetoSelecionado.PrimitivaTipo == PrimitiveType.LineLoop)
                 {
                  objetoSelecionado.PrimitivaTipo = PrimitiveType.LineStrip;
                 } else {
                  objetoSelecionado.PrimitivaTipo = PrimitiveType.LineLoop;
                 }
            }
      }
      if (estadoTeclado.IsKeyPressed(Keys.R))
      {
            if (objetoSelecionado != null)
            {
                  objetoSelecionado.ShaderObjeto = _shaderVermelha;
            }
      }
      if (estadoTeclado.IsKeyPressed(Keys.G))
      {
            if (objetoSelecionado != null)
            {
                  objetoSelecionado.ShaderObjeto = _shaderVerde;
            }
      }
      if (estadoTeclado.IsKeyPressed(Keys.B))
      {
            if (objetoSelecionado != null)
            {
                  objetoSelecionado.ShaderObjeto = _shaderAzul;
            }
      }
      if (estadoTeclado.IsKeyPressed(Keys.Down))
      {
            if (objetoSelecionado != null)
            {
                  objetoSelecionado.MatrizTranslacaoXYZ(0.0, -0.05, 0.0);
            }
      }
      if (estadoTeclado.IsKeyPressed(Keys.Up))
      {
            if (objetoSelecionado != null)
            {
                  objetoSelecionado.MatrizTranslacaoXYZ(0.0, 0.05, 0.0);
            }
      }
      if (estadoTeclado.IsKeyPressed(Keys.Right))
      {
            if (objetoSelecionado != null)
            {
                  objetoSelecionado.MatrizTranslacaoXYZ(0.05, 0.0, 0.0);
            }
      }
      if (estadoTeclado.IsKeyPressed(Keys.Left))
      {
            if (objetoSelecionado != null)
            {
                  objetoSelecionado.MatrizTranslacaoXYZ(-0.05, 0.0, 0.0);
            }
      }
      if (estadoTeclado.IsKeyPressed(Keys.Home))
      {
            if (objetoSelecionado != null)
            {
                  objetoSelecionado.MatrizEscalaXYZBBox(1.5, 1.5, 1.5);
            }
      }
      if (estadoTeclado.IsKeyPressed(Keys.End))
      {
            if (objetoSelecionado != null)
            {
                  objetoSelecionado.MatrizEscalaXYZBBox(0.6667, 0.6667, 0.6667);
            }
      }
      if (estadoTeclado.IsKeyPressed(Keys.D3))
      {
            if (objetoSelecionado != null)
            {
                  objetoSelecionado.MatrizRotacaoZBBox(15);
            }
      }
      if (estadoTeclado.IsKeyPressed(Keys.D4))
      {
            if (objetoSelecionado != null)
            {
                  objetoSelecionado.MatrizRotacaoZBBox(-15);
            }
      }
      if (unfinishedPolygon == null && estadoTeclado.IsKeyPressed(Keys.N))
      {
            objetoSelecionado = Grafocena.GrafoCenaProximo(mundo, objetoSelecionado, grafoLista);
      }
      if (estadoTeclado.IsKeyDown(Keys.Escape))
        Close();
      #endregion

      #region Mouse
      if (MouseState.IsButtonPressed(MouseButton.Right))
      {
            if (unfinishedPolygon == null)
            {
                  List<Ponto4D> polygonPoints = [getMousePos()];
                  Poligono newPolygon = new(objetoSelecionado ?? mundo, ref rotuloAtual, polygonPoints);
                  unfinishedPolygon = newPolygon;
                  Grafocena.GrafocenaAtualizar(mundo, grafoLista);
            } else {
                  unfinishedPolygon.PontosAdicionar(getMousePos());
                  unfinishedPolygon.ObjetoAtualizar();
            }
      }
      if (unfinishedPolygon == null && MouseState.IsButtonPressed(MouseButton.Left))
      {
            objetoSelecionado = null;
            Ponto4D mousePos = getMousePos();
            foreach (KeyValuePair<char, Objeto> objeto in grafoLista)
            {
                  objeto.Value.ScanLine(mousePos, ref objetoSelecionado);
            }
      }
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
      GL.DeleteProgram(_shaderAmarela.Handle);

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