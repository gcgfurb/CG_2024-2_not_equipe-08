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
    private Ponto4D pontoSelecionado = null;
    private List<Ponto> point_list = new List<Ponto>();
    private List<Ponto4D> point4D_list = new List<Ponto4D>();
    private List<SegReta> segReta_list = new List<SegReta>();
    private int point_list_index = 0;

    private readonly float[] _sruEixos =
    [
       0.0f,  0.0f,  0.0f, /* X- */      0.5f,  0.0f,  0.0f, /* X+ */
       0.0f,  0.0f,  0.0f, /* Y- */      0.0f,  0.5f,  0.0f, /* Y+ */
       0.0f,  0.0f,  0.0f, /* Z- */      0.0f,  0.0f,  0.5f  /* Z+ */
    ];
    private int _vertexBufferObject_sruEixos;
    private int _vertexArrayObject_sruEixos;

    private Shader _shaderVermelha;
    private Shader _shaderVerde;
    private Shader _shaderAzul;
    private Shader _shaderCiano;
    private Shader _shaderBranca;

    private bool mouseMovtoPrimeiro = true;
    private Ponto4D mouseMovtoUltimo;

    public Mundo(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
      : base(gameWindowSettings, nativeWindowSettings)
    {
      mundo ??= new Objeto(null, ref rotuloAtual); //padrão Singleton
    }

    protected override void OnLoad()
    {
      base.OnLoad();

      GL.ClearColor(0.3f, 0.1f, 0.3f, 1.0f);

      _shaderVermelha = new Shader("Shaders/shader.vert", "Shaders/shaderVermelha.frag");
      _shaderVerde = new Shader("Shaders/shader.vert", "Shaders/shaderVerde.frag");
      _shaderAzul = new Shader("Shaders/shader.vert", "Shaders/shaderAzul.frag");
      _shaderCiano = new Shader("Shaders/shader.vert", "Shaders/shaderCiano.frag");
      _shaderBranca = new Shader("Shaders/shader.vert", "Shaders/shaderBranca.frag"); 

      #region Eixos: SRU  
      _vertexBufferObject_sruEixos = GL.GenBuffer();
      GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject_sruEixos);
      GL.BufferData(BufferTarget.ArrayBuffer, _sruEixos.Length * sizeof(float), _sruEixos, BufferUsageHint.StaticDraw);
      _vertexArrayObject_sruEixos = GL.GenVertexArray();
      GL.BindVertexArray(_vertexArrayObject_sruEixos);
      GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
      GL.EnableVertexAttribArray(0);
      #endregion

      point4D_list.Add(new Ponto4D(0.5, -0.5));
      point4D_list.Add(new Ponto4D(0.5, 0.5));
      point4D_list.Add(new Ponto4D(-0.5, 0.5));
      point4D_list.Add(new Ponto4D(-0.5, -0.5));

      pontoSelecionado = point4D_list[3];

      #region Objeto: Ponto
      point_list.Add(new Ponto(mundo, ref rotuloAtual, point4D_list[0]));
      point_list.Add(new Ponto(mundo, ref rotuloAtual, point4D_list[1]));
      point_list.Add(new Ponto(mundo, ref rotuloAtual, point4D_list[2]));
      point_list.Add(new Ponto(mundo, ref rotuloAtual, point4D_list[3]));
      point_list[3].ShaderObjeto = _shaderVermelha;
      #endregion

      #region Objeto: SegReta
      SegReta segreta = new SegReta(mundo, ref rotuloAtual, point_list[0].ponto, point_list[1].ponto);
      segreta.ShaderObjeto = _shaderCiano;
      segReta_list.Add(segreta);
      segreta = new SegReta(mundo, ref rotuloAtual, point_list[1].ponto, point_list[2].ponto);
      segreta.ShaderObjeto = _shaderCiano;
      segReta_list.Add(segreta);
      segreta = new SegReta(mundo, ref rotuloAtual, point_list[2].ponto, point_list[3].ponto);
      segreta.ShaderObjeto = _shaderCiano;
      segReta_list.Add(segreta);
      #endregion

      #region Objeto: Spline
      Spline spline = new Spline(mundo, ref rotuloAtual);
      spline.AtualizarSpline(point4D_list[3], true);
      spline.AtualizarSpline(point4D_list[2], true);
      spline.AtualizarSpline(point4D_list[1], true);
      spline.AtualizarSpline(point4D_list[0], false);
      objetoSelecionado = spline;
      #endregion
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

    private void UpdateComponents() {
      int index = point4D_list.IndexOf(pontoSelecionado);
      point_list[index].Atualizar();
      if (index < 3) {
        segReta_list[index].ObjetoAtualizar();
      }
      if (index > 0) {
        segReta_list[index - 1].ObjetoAtualizar();
      }
      ((Spline) objetoSelecionado).Atualizar();
    }

    protected override void OnUpdateFrame(FrameEventArgs e)
    {
      base.OnUpdateFrame(e);

      #region Teclado
      var estadoTeclado = KeyboardState;
      if (estadoTeclado.IsKeyDown(Keys.Escape))
        Close();
      if (estadoTeclado.IsKeyPressed(Keys.C)) {
        pontoSelecionado.Y += 0.05f;
        UpdateComponents();
      }
      if (estadoTeclado.IsKeyPressed(Keys.B)) {
        pontoSelecionado.Y -= 0.05f;
        UpdateComponents();
      }
      if (estadoTeclado.IsKeyPressed(Keys.E)) {
        pontoSelecionado.X -= 0.05f;
        UpdateComponents();
      }
      if (estadoTeclado.IsKeyPressed(Keys.D)) {
        pontoSelecionado.X += 0.05f;
        UpdateComponents();
      }
      if (estadoTeclado.IsKeyPressed(Keys.Space)) {
        int index = point4D_list.IndexOf(pontoSelecionado);
        point_list[index].ShaderObjeto =  _shaderBranca;
        pontoSelecionado = point4D_list[point_list_index];
        point_list[point_list_index].ShaderObjeto = _shaderVermelha;
        point_list_index = (point_list_index + 1)%point_list.Count;
      }
      if (estadoTeclado.IsKeyPressed(Keys.Equal)) {
        ((Spline) objetoSelecionado).SplineQtdPto(1);
      }
      if (estadoTeclado.IsKeyPressed(Keys.Minus)) {
        ((Spline) objetoSelecionado).SplineQtdPto(-1);
      }
      if (estadoTeclado.IsKeyPressed(Keys.G))
        Grafocena.GrafoCenaImprimir(mundo, grafoLista);
      if (estadoTeclado.IsKeyPressed(Keys.P))
      {
        if (objetoSelecionado != null)
          Console.WriteLine(objetoSelecionado);
        else
          Console.WriteLine("objetoSelecionado: MUNDO \n__________________________________\n");
      }
      if (estadoTeclado.IsKeyPressed(Keys.Right) && objetoSelecionado != null)
      {
        if (objetoSelecionado.PontosListaTamanho > 0)
        {
          objetoSelecionado.PontosAlterar(new Ponto4D(objetoSelecionado.PontosId(0).X + 0.005, objetoSelecionado.PontosId(0).Y, 0), 0);
          objetoSelecionado.ObjetoAtualizar();
        }
      }

      if (estadoTeclado.IsKeyPressed(Keys.R) && objetoSelecionado != null)
      {
        //FIXME: Spline limpa os pontos da Spline, mas não limpa pontos e poliedro de controle 
        objetoSelecionado.PontosApagar();
      }
      #endregion

      #region  Mouse
      int janelaLargura = ClientSize.X;
      int janelaAltura = ClientSize.Y;
      Ponto4D mousePonto = new(MousePosition.X, MousePosition.Y);
      Ponto4D sruPonto = Utilitario.NDC_TelaSRU(janelaLargura, janelaAltura, mousePonto);

      if (estadoTeclado.IsKeyPressed(Keys.LeftShift))
      {
        if (mouseMovtoPrimeiro)
        {
          mouseMovtoUltimo = sruPonto;
          mouseMovtoPrimeiro = false;
        }
        else
        {
          var deltaX = sruPonto.X - mouseMovtoUltimo.X;
          var deltaY = sruPonto.Y - mouseMovtoUltimo.Y;
          mouseMovtoUltimo = sruPonto;

          objetoSelecionado.PontosAlterar(new Ponto4D(objetoSelecionado.PontosId(0).X + deltaX, objetoSelecionado.PontosId(0).Y + deltaY, 0), 0);
          objetoSelecionado.ObjetoAtualizar();
        }
      }
      if (estadoTeclado.IsKeyDown(Keys.LeftShift))
      {
        objetoSelecionado.PontosAlterar(sruPonto, 0);
        objetoSelecionado.ObjetoAtualizar();
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