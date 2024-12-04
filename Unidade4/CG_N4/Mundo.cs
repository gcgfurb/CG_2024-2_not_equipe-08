#define CG_DEBUG
#define CG_Gizmo      
#define CG_OpenGL      
// #define CG_OpenTK
// #define CG_DirectX      
// #define CG_Privado      

using CG_Biblioteca;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Desktop;
using System;
using OpenTK.Mathematics;
using System.Collections.Generic;

//FIXME: padrão Singleton

namespace gcgcg
{
  public class Mundo : GameWindow
  {
    private static Objeto mundo = null;
    private char rotuloNovo = '?';
    private Objeto objetoSelecionado = null;

    private Cubo cuboMenor = null;

    private readonly float[] _sruEixos =
    {
      -0.5f,  0.0f,  0.0f, /* X- */      0.5f,  0.0f,  0.0f, /* X+ */
       0.0f, -0.5f,  0.0f, /* Y- */      0.0f,  0.5f,  0.0f, /* Y+ */
       0.0f,  0.0f, -0.5f, /* Z- */      0.0f,  0.0f,  0.5f  /* Z+ */
    };

    private int _vertexBufferObject_sruEixos;
    private int _vertexArrayObject_sruEixos;

    private Shader _shaderBranca;
    private Shader _shaderVermelha;
    private Shader _shaderVerde;
    private Shader _shaderAzul;
    private Shader _shaderCiano;
    private Shader _shaderMagenta;
    private Shader _shaderAmarela;
    private Shader _shaderTresMosqueteiros;
   // private Shader _shaderFrontFaceTexture;
   // private Shader _shaderBackFaceTexture;
   // private Shader _shaderTopFaceTexture;
   // private Shader _shaderBottomFaceTexture;
   // private Shader _shaderRightFaceTexture;
   // private Shader _shaderLeftFaceTexture;
    private Texture _textureMosqueteiros;
    private List<float[]> _faceVertices;
    private List<uint[]> _faceIndices;
    private readonly List<Ponto4D> _rectangleFirstPoints = new List<Ponto4D> {
      new Ponto4D(-1.0, -1.0, 1.0),
      new Ponto4D(-1.0, -1.0, -1.0),
      new Ponto4D(-1.0, 1.0, -1.0),
      new Ponto4D(-1.0, -1.0, -1.0),
      new Ponto4D(1.0, -1.0, -1.0),
      new Ponto4D(-1.0, -1.0, -1.0)
    };
    private readonly List<Ponto4D> _rectangleSecondPoints = new List<Ponto4D> {
      new Ponto4D(1.0, 1.0, 1.0),
      new Ponto4D(1.0, 1.0, -1.0),
      new Ponto4D(1.0, 1.0, 1.0),
      new Ponto4D(1.0, -1.0, 1.0),
      new Ponto4D(1.0, 1.0, 1.0),
      new Ponto4D(-1.0, 1.0, 1.0)
    };
    //private List<Shader> _shadersWithTextures;
    private List<int> _vertexBufferObjects_texture;
    private List<int> _vertexArrayObjects_texture;
    private List<int> _elementBufferObjects_texture;

    private readonly int _vertexBufferObject_texture_frontFace = 0;
    private readonly int _vertexBufferObject_texture_backFace = 0;
    private readonly int _vertexBufferObject_texture_topFace = 0;
    private readonly int _vertexBufferObject_texture_bottomFace = 0;
    private readonly int _vertexBufferObject_texture_rightFace = 0;
    private readonly int _vertexBufferObject_texture_leftFace = 0;

    private readonly int _vertexArrayObject_texture_frontFace = 0;
    private readonly int _vertexArrayObject_texture_backFace = 0;
    private readonly int _vertexArrayObject_texture_topFace = 0;
    private readonly int _vertexArrayObject_texture_bottomFace = 0;
    private readonly int _vertexArrayObject_texture_rightFace = 0;
    private readonly int _vertexArrayObject_texture_leftFace = 0;

    private readonly int _elementBufferObject_texture_frontFace = 0;
    private readonly int _elementBufferObject_texture_backFace = 0;
    private readonly int _elementBufferObject_texture_topFace = 0;
    private readonly int _elementBufferObject_texture_bottomFace = 0;
    private readonly int _elementBufferObject_texture_rightFace = 0;
    private readonly int _elementBufferObject_texture_leftFace = 0;

    private int _vertexBufferObject_light;
    private readonly Vector3 _lightPos = new Vector3(1.2f, 1.0f, 2.0f);
    private int _vaoModel;
    private int _vaoLamp;
    private Shader _lampShader;
    private Shader _lightingShader;

    private readonly float[] _vertices =
        {
            // Positions          Normals              Texture coords
            -0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  0.0f, 0.0f,
             0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  1.0f, 0.0f,
             0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  1.0f, 1.0f,
             0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  1.0f, 1.0f,
            -0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  0.0f, 1.0f,
            -0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  0.0f, 0.0f,

            -0.5f, -0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  0.0f, 0.0f,
             0.5f, -0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  1.0f, 0.0f,
             0.5f,  0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  1.0f, 1.0f,
             0.5f,  0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  1.0f, 1.0f,
            -0.5f,  0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  0.0f, 1.0f,
            -0.5f, -0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  0.0f, 0.0f,

            //-0.5f,  0.5f,  0.5f, -1.0f,  0.0f,  0.0f,  1.0f, 0.0f,
            //-0.5f,  0.5f, -0.5f, -1.0f,  0.0f,  0.0f,  1.0f, 1.0f,
            //-0.5f, -0.5f, -0.5f, -1.0f,  0.0f,  0.0f,  0.0f, 1.0f,
            //-0.5f, -0.5f, -0.5f, -1.0f,  0.0f,  0.0f,  0.0f, 1.0f,
            //-0.5f, -0.5f,  0.5f, -1.0f,  0.0f,  0.0f,  0.0f, 0.0f,
            //-0.5f,  0.5f,  0.5f, -1.0f,  0.0f,  0.0f,  1.0f, 0.0f,

            -0.5f,  0.5f, -0.5f, -1.0f,  0.0f,  0.0f,  0.0f, 1.0f,
            -0.5f,  0.5f,  0.5f, -1.0f,  0.0f,  0.0f,  1.0f, 1.0f,
            -0.5f, -0.5f,  0.5f, -1.0f,  0.0f,  0.0f,  1.0f, 0.0f,
            -0.5f, -0.5f,  0.5f, -1.0f,  0.0f,  0.0f,  1.0f, 0.0f,
            -0.5f, -0.5f, -0.5f, -1.0f,  0.0f,  0.0f,  0.0f, 0.0f,
            -0.5f,  0.5f, -0.5f, -1.0f,  0.0f,  0.0f,  0.0f, 1.0f,

             //0.5f,  0.5f,  0.5f,  1.0f,  0.0f,  0.0f,  1.0f, 0.0f,
             //0.5f,  0.5f, -0.5f,  1.0f,  0.0f,  0.0f,  1.0f, 1.0f,
             //0.5f, -0.5f, -0.5f,  1.0f,  0.0f,  0.0f,  0.0f, 1.0f,
             //0.5f, -0.5f, -0.5f,  1.0f,  0.0f,  0.0f,  0.0f, 1.0f,
             //0.5f, -0.5f,  0.5f,  1.0f,  0.0f,  0.0f,  0.0f, 0.0f,
             //0.5f,  0.5f,  0.5f,  1.0f,  0.0f,  0.0f,  1.0f, 0.0f,

             0.5f,  0.5f,  0.5f,  1.0f,  0.0f,  0.0f,  0.0f, 1.0f,
             0.5f,  0.5f, -0.5f,  1.0f,  0.0f,  0.0f,  1.0f, 1.0f,
             0.5f, -0.5f, -0.5f,  1.0f,  0.0f,  0.0f,  1.0f, 0.0f,
             0.5f, -0.5f, -0.5f,  1.0f,  0.0f,  0.0f,  1.0f, 0.0f,
             0.5f, -0.5f,  0.5f,  1.0f,  0.0f,  0.0f,  0.0f, 0.0f,
             0.5f,  0.5f,  0.5f,  1.0f,  0.0f,  0.0f,  0.0f, 1.0f,

            -0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,  0.0f, 1.0f,
             0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,  1.0f, 1.0f,
             0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,  1.0f, 0.0f,
             0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,  1.0f, 0.0f,
            -0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,  0.0f, 0.0f,
            -0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,  0.0f, 1.0f,

            -0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,  0.0f, 1.0f,
             0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,  1.0f, 1.0f,
             0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,  1.0f, 0.0f,
             0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,  1.0f, 0.0f,
            -0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,  0.0f, 0.0f,
            -0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,  0.0f, 1.0f
        };
    private Camera _camera;

    public Mundo(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
           : base(gameWindowSettings, nativeWindowSettings)
    {
      mundo ??= new Objeto(null, ref rotuloNovo); //padrão Singleton
    }


    protected override void OnLoad()
    {
      base.OnLoad();

      Utilitario.Diretivas();
#if CG_DEBUG      
      Console.WriteLine("Tamanho interno da janela de desenho: " + ClientSize.X + "x" + ClientSize.Y);
#endif

      GL.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);

      GL.Enable(EnableCap.DepthTest);

      #region Shaders
      _shaderBranca = new Shader("Shaders/shader.vert", "Shaders/shaderBranca.frag");
      _shaderVermelha = new Shader("Shaders/shader.vert", "Shaders/shaderVermelha.frag");
      _shaderVerde = new Shader("Shaders/shader.vert", "Shaders/shaderVerde.frag");
      _shaderAzul = new Shader("Shaders/shader.vert", "Shaders/shaderAzul.frag");
      _shaderCiano = new Shader("Shaders/shader.vert", "Shaders/shaderCiano.frag");
      _shaderMagenta = new Shader("Shaders/shader.vert", "Shaders/shaderMagenta.frag");
      _shaderAmarela = new Shader("Shaders/shader.vert", "Shaders/shaderAmarela.frag");
      _shaderTresMosqueteiros = new Shader("Shaders/shaderTexture.vert", "Shaders/shaderTexture.frag");
      //_shaderFrontFaceTexture = new Shader("Shaders/shaderTexture.vert", "Shaders/shaderTexture.frag");
      //_shaderBackFaceTexture = new Shader("Shaders/shaderTexture.vert", "Shaders/shaderTexture.frag");
      //_shaderTopFaceTexture = new Shader("Shaders/shaderTexture.vert", "Shaders/shaderTexture.frag");
      //_shaderBottomFaceTexture = new Shader("Shaders/shaderTexture.vert", "Shaders/shaderTexture.frag");
      //_shaderRightFaceTexture = new Shader("Shaders/shaderTexture.vert", "Shaders/shaderTexture.frag");
      //_shaderLeftFaceTexture = new Shader("Shaders/shaderTexture.vert", "Shaders/shaderTexture.frag");
      #endregion

      #region Eixos: SRU  
      _vertexBufferObject_sruEixos = GL.GenBuffer();
      GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject_sruEixos);
      GL.BufferData(BufferTarget.ArrayBuffer, _sruEixos.Length * sizeof(float), _sruEixos, BufferUsageHint.StaticDraw);
      _vertexArrayObject_sruEixos = GL.GenVertexArray();
      GL.BindVertexArray(_vertexArrayObject_sruEixos);
      GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
      GL.EnableVertexAttribArray(0);
      #endregion

      #region Objeto: Cubo
      cuboMenor = new Cubo(mundo, ref rotuloNovo);
      cuboMenor.MatrizEscalaXYZ(0.1, 0.1, 0.1);
      cuboMenor.MatrizTranslacaoXYZ(0, 0, 3);
      cuboMenor.TrocaEixoRotacao('y');

      objetoSelecionado = cuboMenor;
      //objetoSelecionado = new Cubo(mundo, ref rotuloNovo);
      //objetoSelecionado.shaderCor = _shaderAmarela;
      //_textureMosqueteiros = Texture.LoadFromFile("Textures/tres_mosqueteiros.png");
      //_faceVertices = ((Cubo) objetoSelecionado).GetFaceVertices();
      //_faceIndices = ((Cubo) objetoSelecionado).GetFaceIndices();

      _vertexArrayObjects_texture = [
        _vertexArrayObject_texture_frontFace,
        _vertexArrayObject_texture_backFace,
        _vertexArrayObject_texture_topFace,
        _vertexArrayObject_texture_bottomFace,
        _vertexArrayObject_texture_rightFace,
        _vertexArrayObject_texture_leftFace
      ];

      _vertexBufferObjects_texture = [
        _vertexBufferObject_texture_frontFace,
        _vertexBufferObject_texture_backFace,
        _vertexBufferObject_texture_topFace,
        _vertexBufferObject_texture_bottomFace,
        _vertexBufferObject_texture_rightFace,
        _vertexBufferObject_texture_leftFace
      ];

      _elementBufferObjects_texture = [
        _elementBufferObject_texture_frontFace,
        _elementBufferObject_texture_backFace,
        _elementBufferObject_texture_topFace,
        _elementBufferObject_texture_bottomFace,
        _elementBufferObject_texture_rightFace,
        _elementBufferObject_texture_leftFace
      ];

      //_shadersWithTextures = [
      //  _shaderFrontFaceTexture,
      //  _shaderBackFaceTexture,
      //  _shaderTopFaceTexture,
      //  _shaderBottomFaceTexture,
      //  _shaderRightFaceTexture,
      //  _shaderLeftFaceTexture
      //];

      _lightingShader = new Shader("Shaders/shaderLighting.vert", "Shaders/lighting.frag");
      _lampShader = new Shader("Shaders/shaderLighting.vert", "Shaders/shaderLighting.frag");

      OnLoadTextureLight();
      //OnLoadUseTextures();
      //OnLoadUseLight();

      #endregion
      // objetoSelecionado.MatrizEscalaXYZ(0.2, 0.2, 0.2);

      _camera = new Camera(Vector3.UnitZ * 5, ClientSize.X / (float)ClientSize.Y);
    }

    protected override void OnRenderFrame(FrameEventArgs e)
    {
      base.OnRenderFrame(e);

      GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

      mundo.Desenhar(new Transformacao4D(), _camera);
      OnRenderTextureLight();
      //OnRenderFrameUseTextures();
      //OnRenderFrameUseLight();

#if CG_Gizmo      
      Gizmo_Sru3D();
#endif
      SwapBuffers();
    }

    protected void OnRenderTextureLight()
    {
      GL.BindVertexArray(_vaoModel);

      _textureMosqueteiros.Use(TextureUnit.Texture0);
      _lightingShader.Use();

      _lightingShader.SetMatrix4("model", Matrix4.Identity);
      _lightingShader.SetMatrix4("view", _camera.GetViewMatrix());
      _lightingShader.SetMatrix4("projection", _camera.GetProjectionMatrix());

      _lightingShader.SetVector3("viewPos", _camera.Position);

      _lightingShader.SetInt("material.diffuse", 0);
      _lightingShader.SetInt("material.specular", 1);
      _lightingShader.SetVector3("material.specular", new Vector3(0.5f, 0.5f, 0.5f));
      _lightingShader.SetFloat("material.shininess", 32.0f);

      _lightingShader.SetVector3("light.position", _lightPos);
      _lightingShader.SetVector3("light.ambient", new Vector3(0.2f));
      _lightingShader.SetVector3("light.diffuse", new Vector3(0.5f));
      _lightingShader.SetVector3("light.specular", new Vector3(1.0f));

      GL.DrawArrays(PrimitiveType.Triangles, 0, 36);

      GL.BindVertexArray(_vaoLamp);
      
      _lampShader.Use();

      Matrix4 lampMatrix = Matrix4.Identity;
      lampMatrix *= Matrix4.CreateScale(0.2f);
      lampMatrix *= Matrix4.CreateTranslation(_lightPos);

      _lampShader.SetMatrix4("model", lampMatrix);
      _lampShader.SetMatrix4("view", _camera.GetViewMatrix());
      _lampShader.SetMatrix4("projection", _camera.GetProjectionMatrix());

      GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
    }

    protected void OnRenderFrameUseLight()
    {
      GL.BindVertexArray(_vaoModel);

      _lightingShader.Use();

      _lightingShader.SetMatrix4("model", Matrix4.Identity);
      _lightingShader.SetMatrix4("view", _camera.GetViewMatrix());
      _lightingShader.SetMatrix4("projection", _camera.GetProjectionMatrix());

      _lightingShader.SetVector3("objectColor", new Vector3(1.0f, 1.0f, 1.0f));
      _lightingShader.SetVector3("lightColor", new Vector3(1.0f, 1.0f, 1.0f));
      _lightingShader.SetVector3("lightPos", _lightPos);
      _lightingShader.SetVector3("viewPos", _camera.Position);

      GL.DrawArrays(PrimitiveType.Triangles, 0, 36);

      GL.BindVertexArray(_vaoLamp);

      _lampShader.Use();

      Matrix4 lampMatrix = Matrix4.CreateScale(0.2f);
      lampMatrix = lampMatrix * Matrix4.CreateTranslation(_lightPos);

      _lampShader.SetMatrix4("model", lampMatrix);
      _lampShader.SetMatrix4("view", _camera.GetViewMatrix());
      _lampShader.SetMatrix4("projection", _camera.GetProjectionMatrix());

      GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
    }

    protected void OnRenderFrameUseTextures()
    {
      for (int i = 0; i < _vertexArrayObjects_texture.Count; i++)
      {
        GL.BindVertexArray(_vertexArrayObjects_texture[i]);
        _textureMosqueteiros.Use(TextureUnit.Texture0);
        _shaderTresMosqueteiros.Use();

        GL.DrawElements(PrimitiveType.Triangles, _faceIndices[i].Length, DrawElementsType.UnsignedInt, 0);
      }
    }

    protected override void OnUpdateFrame(FrameEventArgs e)
    {
      base.OnUpdateFrame(e);

      cuboMenor.MatrizRotacao(0.005);

      // ☞ 396c2670-8ce0-4aff-86da-0f58cd8dcfdc   TODO: forma otimizada para teclado.
      #region Teclado
      var estadoTeclado = KeyboardState;
      if (estadoTeclado.IsKeyDown(Keys.Escape))
        Close();
      if (estadoTeclado.IsKeyPressed(Keys.Space))
      {
        if (objetoSelecionado == null)
          objetoSelecionado = mundo;
          objetoSelecionado.shaderCor = _shaderBranca;
          objetoSelecionado = mundo.GrafocenaBuscaProximo(objetoSelecionado);
          objetoSelecionado.shaderCor = _shaderAmarela;
      }
      if (estadoTeclado.IsKeyPressed(Keys.G))
        mundo.GrafocenaImprimir("");
      if (estadoTeclado.IsKeyPressed(Keys.P) && objetoSelecionado != null)
        Console.WriteLine(objetoSelecionado.ToString());
      if (estadoTeclado.IsKeyPressed(Keys.M) && objetoSelecionado != null)
        objetoSelecionado.MatrizImprimir();
      if (estadoTeclado.IsKeyPressed(Keys.I) && objetoSelecionado != null)
        objetoSelecionado.MatrizAtribuirIdentidade();
      if (estadoTeclado.IsKeyPressed(Keys.Left) && objetoSelecionado != null)
        objetoSelecionado.MatrizTranslacaoXYZ(-0.05, 0, 0);
      if (estadoTeclado.IsKeyPressed(Keys.Right) && objetoSelecionado != null)
        objetoSelecionado.MatrizTranslacaoXYZ(0.05, 0, 0);
      if (estadoTeclado.IsKeyPressed(Keys.Up) && objetoSelecionado != null)
        objetoSelecionado.MatrizTranslacaoXYZ(0, 0.05, 0);
      if (estadoTeclado.IsKeyPressed(Keys.Down) && objetoSelecionado != null)
        objetoSelecionado.MatrizTranslacaoXYZ(0, -0.05, 0);
      if (estadoTeclado.IsKeyPressed(Keys.O) && objetoSelecionado != null)
        objetoSelecionado.MatrizTranslacaoXYZ(0, 0, 0.05);
      if (estadoTeclado.IsKeyPressed(Keys.L) && objetoSelecionado != null)
        objetoSelecionado.MatrizTranslacaoXYZ(0, 0, -0.05);
      if (estadoTeclado.IsKeyPressed(Keys.PageUp) && objetoSelecionado != null)
        objetoSelecionado.MatrizEscalaXYZ(2, 2, 2);
      if (estadoTeclado.IsKeyPressed(Keys.PageDown) && objetoSelecionado != null)
        objetoSelecionado.MatrizEscalaXYZ(0.5, 0.5, 0.5);
      if (estadoTeclado.IsKeyPressed(Keys.Home) && objetoSelecionado != null)
        objetoSelecionado.MatrizEscalaXYZBBox(0.5, 0.5, 0.5);
      if (estadoTeclado.IsKeyPressed(Keys.End) && objetoSelecionado != null)
        objetoSelecionado.MatrizEscalaXYZBBox(2, 2, 2);
      if (estadoTeclado.IsKeyPressed(Keys.D1) && objetoSelecionado != null)
        objetoSelecionado.MatrizRotacao(10);
      if (estadoTeclado.IsKeyPressed(Keys.D2) && objetoSelecionado != null)
        objetoSelecionado.MatrizRotacao(-10);
      if (estadoTeclado.IsKeyPressed(Keys.D3) && objetoSelecionado != null)
        objetoSelecionado.MatrizRotacaoZBBox(10);
      if (estadoTeclado.IsKeyPressed(Keys.D4) && objetoSelecionado != null)
        objetoSelecionado.MatrizRotacaoZBBox(-10);

      const float cameraSpeed = 1.5f;
      if (estadoTeclado.IsKeyDown(Keys.Z))
        _camera.Position = Vector3.UnitZ * 5;
      if (estadoTeclado.IsKeyDown(Keys.W))
        _camera.Position += _camera.Front * cameraSpeed * (float)e.Time; // Forward
      if (estadoTeclado.IsKeyDown(Keys.S))
        _camera.Position -= _camera.Front * cameraSpeed * (float)e.Time; // Backwards
      if (estadoTeclado.IsKeyDown(Keys.A))
        _camera.Position -= _camera.Right * cameraSpeed * (float)e.Time; // Left
      if (estadoTeclado.IsKeyDown(Keys.D))
        _camera.Position += _camera.Right * cameraSpeed * (float)e.Time; // Right
      if (estadoTeclado.IsKeyDown(Keys.RightShift))
        _camera.Position += _camera.Up * cameraSpeed * (float)e.Time; // Up
      if (estadoTeclado.IsKeyDown(Keys.LeftShift))
        _camera.Position -= _camera.Up * cameraSpeed * (float)e.Time; // Down
      // if (estadoTeclado.IsKeyDown(Keys.D9))
      //   _camera.Position += _camera.Up * cameraSpeed * (float)e.Time; // Up
      // if (estadoTeclado.IsKeyDown(Keys.D0))
      //   _camera.Position -= _camera.Up * cameraSpeed * (float)e.Time; // Down

      #endregion

      #region  Mouse

      if (MouseState.IsButtonPressed(MouseButton.Left))
      {
        Console.WriteLine("MouseState.IsButtonPressed(MouseButton.Left)");
        Console.WriteLine("__ Valores do Espaço de Tela");
        Console.WriteLine("Vector2 mousePosition: " + MousePosition);
        Console.WriteLine("Vector2i windowSize: " + ClientSize);
      }
      if (MouseState.IsButtonDown(MouseButton.Right) && objetoSelecionado != null)
      {
        Console.WriteLine("MouseState.IsButtonDown(MouseButton.Right)");

        int janelaLargura = ClientSize.X;
        int janelaAltura = ClientSize.Y;
        Ponto4D mousePonto = new Ponto4D(MousePosition.X, MousePosition.Y);
        Ponto4D sruPonto = Utilitario.NDC_TelaSRU(janelaLargura, janelaAltura, mousePonto);

        objetoSelecionado.PontosAlterar(sruPonto, 0);
      }
      if (MouseState.IsButtonReleased(MouseButton.Right))
      {
        Console.WriteLine("MouseState.IsButtonReleased(MouseButton.Right)");
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

      GL.DeleteBuffer(_vertexBufferObject_sruEixos);
      GL.DeleteVertexArray(_vertexArrayObject_sruEixos);

      OnUnloadDeleteArrays();
      OnUnloadDeleteBuffers();
      OnUnloadDeleteElementBuffers();

      GL.DeleteProgram(_shaderBranca.Handle);
      GL.DeleteProgram(_shaderVermelha.Handle);
      GL.DeleteProgram(_shaderVerde.Handle);
      GL.DeleteProgram(_shaderAzul.Handle);
      GL.DeleteProgram(_shaderCiano.Handle);
      GL.DeleteProgram(_shaderMagenta.Handle);
      GL.DeleteProgram(_shaderAmarela.Handle);
      GL.DeleteProgram(_shaderTresMosqueteiros.Handle);
      //GL.DeleteProgram(_shaderFrontFaceTexture.Handle);
      //GL.DeleteProgram(_shaderBackFaceTexture.Handle);
      //GL.DeleteProgram(_shaderTopFaceTexture.Handle);
      //GL.DeleteProgram(_shaderBottomFaceTexture.Handle);
      //GL.DeleteProgram(_shaderRightFaceTexture.Handle);
      //GL.DeleteProgram(_shaderLeftFaceTexture.Handle);
      GL.DeleteProgram(_lightingShader.Handle);
      GL.DeleteProgram(_lampShader.Handle);


      base.OnUnload();
    }

    protected void OnUnloadDeleteArrays()
    {
      foreach (int _vertexArrayObject_texture in _vertexArrayObjects_texture)
      {
        GL.DeleteVertexArray(_vertexArrayObject_texture);
      }
    }

    protected void OnUnloadDeleteBuffers()
    {
      foreach (int _vertexBufferObject_texture in _vertexBufferObjects_texture)
      {
        GL.DeleteBuffer(_vertexBufferObject_texture);
      }
    }

    protected void OnUnloadDeleteElementBuffers()
    {
      foreach (int _elementBufferObject_texture in _elementBufferObjects_texture)
      {
        GL.DeleteBuffer(_elementBufferObject_texture);
      }
    }

    protected void OnLoadTextureLight()
    {
      _vertexBufferObject_light = GL.GenBuffer();
      GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject_light);
      GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.StaticDraw);

      {
        _vaoModel = GL.GenVertexArray();
        GL.BindVertexArray(_vaoModel);

        // All of the vertex attributes have been updated to now have a stride of 8 float sizes.
        var positionLocation = _lightingShader.GetAttribLocation("aPos");
        GL.EnableVertexAttribArray(positionLocation);
        GL.VertexAttribPointer(positionLocation, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 0);

        var normalLocation = _lightingShader.GetAttribLocation("aNormal");
        GL.EnableVertexAttribArray(normalLocation);
        GL.VertexAttribPointer(normalLocation, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 3 * sizeof(float));

        // The texture coords have now been added too, remember we only have 2 coordinates as the texture is 2d,
        // so the size parameter should only be 2 for the texture coordinates.
        var texCoordLocation = _lightingShader.GetAttribLocation("aTexCoords");
        GL.EnableVertexAttribArray(texCoordLocation);
        GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 8 * sizeof(float), 6 * sizeof(float));
      }
            
      {
          _vaoLamp = GL.GenVertexArray();
          GL.BindVertexArray(_vaoLamp);

          // The lamp shader should have its stride updated aswell, however we dont actually
          // use the texture coords for the lamp, so we dont need to add any extra attributes.
          var positionLocation = _lampShader.GetAttribLocation("aPos");
          GL.EnableVertexAttribArray(positionLocation);
          GL.VertexAttribPointer(positionLocation, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 0);
      }

      _textureMosqueteiros = Texture.LoadFromFile("Textures/tres_mosqueteiros.png");
    }

    protected void OnLoadUseTextures()
    {
      for (int i = 0; i < _vertexArrayObjects_texture.Count; i++)
      {
        GL.Enable(EnableCap.Texture2D);
        _vertexArrayObjects_texture[i] = GL.GenVertexArray();
        GL.BindVertexArray(_vertexArrayObjects_texture[i]);

        _vertexBufferObjects_texture[i] = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObjects_texture[i]);
        GL.BufferData(BufferTarget.ArrayBuffer, _faceVertices[i].Length * sizeof(float), _faceVertices[i], BufferUsageHint.StaticDraw);

        _elementBufferObjects_texture[i] = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObjects_texture[i]);
        GL.BufferData(BufferTarget.ElementArrayBuffer, _faceIndices[i].Length * sizeof(uint), _faceIndices[i], BufferUsageHint.StaticDraw);

        _shaderTresMosqueteiros.Use();

        var vertexLocation = _shaderTresMosqueteiros.GetAttribLocation("aPosition");
        GL.EnableVertexAttribArray(vertexLocation);
        GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);

        var texCoordLocation = _shaderTresMosqueteiros.GetAttribLocation("aTexCoord");
        GL.EnableVertexAttribArray(texCoordLocation);
        GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));

        _textureMosqueteiros.Use(TextureUnit.Texture0);

        Retangulo faceRectangle = new Retangulo(objetoSelecionado, ref rotuloNovo, _rectangleFirstPoints[i], _rectangleSecondPoints[i], true);
        faceRectangle.shaderCor = _shaderTresMosqueteiros;
      }
    }

    private void OnLoadUseLight()
    {
      //for (int i = 0; i < _faceVertices.Count; i++)
      //{
        _vertexBufferObject_light = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject_light);
        GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.StaticDraw);

        {
          _vaoModel = GL.GenVertexArray();
          GL.BindVertexArray(_vaoModel);

          var positionLocation = _lightingShader.GetAttribLocation("aPos");
          GL.EnableVertexAttribArray(positionLocation);
          // Remember to change the stride as we now have 6 floats per vertex
          GL.VertexAttribPointer(positionLocation, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
          
          // We now need to define the layout of the normal so the shader can use it
          var normalLocation = _lightingShader.GetAttribLocation("aNormal");
          GL.EnableVertexAttribArray(normalLocation);
          GL.VertexAttribPointer(normalLocation, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
        }

        {
          _vaoLamp = GL.GenVertexArray();
          GL.BindVertexArray(_vaoLamp);
          
          var positionLocation = _lampShader.GetAttribLocation("aPos");
          GL.EnableVertexAttribArray(positionLocation);
          
          GL.VertexAttribPointer(positionLocation, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
        }
      //}
    }

#if CG_Gizmo
    private void Gizmo_Sru3D()
    {
#if CG_OpenGL && !CG_DirectX
      var model = Matrix4.Identity;
      GL.BindVertexArray(_vertexArrayObject_sruEixos);
      // EixoX
      _shaderVermelha.SetMatrix4("model", model);
      _shaderVermelha.SetMatrix4("view", _camera.GetViewMatrix());
      _shaderVermelha.SetMatrix4("projection", _camera.GetProjectionMatrix());
      _shaderVermelha.Use();
      GL.DrawArrays(PrimitiveType.Lines, 0, 2);
      // EixoY
      _shaderVerde.SetMatrix4("model", model);
      _shaderVerde.SetMatrix4("view", _camera.GetViewMatrix());
      _shaderVerde.SetMatrix4("projection", _camera.GetProjectionMatrix());
      _shaderVerde.Use();
      GL.DrawArrays(PrimitiveType.Lines, 2, 2);
      // EixoZ
      _shaderAzul.SetMatrix4("model", model);
      _shaderAzul.SetMatrix4("view", _camera.GetViewMatrix());
      _shaderAzul.SetMatrix4("projection", _camera.GetProjectionMatrix());
      _shaderAzul.Use();
      GL.DrawArrays(PrimitiveType.Lines, 4, 2);
#elif CG_DirectX && !CG_OpenGL
      Console.WriteLine(" .. Coloque aqui o seu código em DirectX");
#elif (CG_DirectX && CG_OpenGL) || (!CG_DirectX && !CG_OpenGL)
      Console.WriteLine(" .. ERRO de Render - escolha OpenGL ou DirectX !!");
#endif
    }
#endif    

  }
}
