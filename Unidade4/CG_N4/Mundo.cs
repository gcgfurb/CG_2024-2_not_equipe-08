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
using System.Drawing;

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
    private Texture _textureMosqueteiros;
    private int _vertexBufferObject_light;
    private int _elementBufferObject;
    private int _vertexBufferObject;
    private int _vertexArrayObject;
    private readonly Vector3 _lightPos = new Vector3(1.2f, 1.0f, 2.0f);
    private int _vaoModel;
    private int _vaoLamp;
    private Shader _lampShader;
    private Shader _lightingShader;
    private Shader _basicLightingShader;
    private Shader _lightingMapShader;
    private Shader _lightCasterDirLightsShader;
    private Shader _lightCasterPointLightShader;
    private Shader _lightCasterSpotlightShader;
    private Shader _multipleLightsShader;
    private int _lightShaderToUse = 2;

    private readonly float[] _vertices =
        {
            // Positions          Normals              Texture coords
             1.0f, -1.0f, -1.0f,  0.0f,  0.0f, -1.0f,  0.0f, 0.0f, // FACE DE TRAS
            -1.0f, -1.0f, -1.0f,  0.0f,  0.0f, -1.0f,  1.0f, 0.0f,
            -1.0f,  1.0f, -1.0f,  0.0f,  0.0f, -1.0f,  1.0f, 1.0f,
            -1.0f,  1.0f, -1.0f,  0.0f,  0.0f, -1.0f,  1.0f, 1.0f,
             1.0f,  1.0f, -1.0f,  0.0f,  0.0f, -1.0f,  0.0f, 1.0f,
             1.0f, -1.0f, -1.0f,  0.0f,  0.0f, -1.0f,  0.0f, 0.0f,

            -1.0f, -1.0f,  1.0f,  0.0f,  0.0f,  1.0f,  0.0f, 0.0f, // FACE FRONTAL
             1.0f, -1.0f,  1.0f,  0.0f,  0.0f,  1.0f,  1.0f, 0.0f,
             1.0f,  1.0f,  1.0f,  0.0f,  0.0f,  1.0f,  1.0f, 1.0f,
             1.0f,  1.0f,  1.0f,  0.0f,  0.0f,  1.0f,  1.0f, 1.0f,
            -1.0f,  1.0f,  1.0f,  0.0f,  0.0f,  1.0f,  0.0f, 1.0f,
            -1.0f, -1.0f,  1.0f,  0.0f,  0.0f,  1.0f,  0.0f, 0.0f,

            -1.0f,  1.0f, -1.0f, -1.0f,  0.0f,  0.0f,  0.0f, 1.0f, // FACE ESQUERDA DO CUBO
            -1.0f,  1.0f,  1.0f, -1.0f,  0.0f,  0.0f,  1.0f, 1.0f,
            -1.0f, -1.0f,  1.0f, -1.0f,  0.0f,  0.0f,  1.0f, 0.0f,
            -1.0f, -1.0f,  1.0f, -1.0f,  0.0f,  0.0f,  1.0f, 0.0f,
            -1.0f, -1.0f, -1.0f, -1.0f,  0.0f,  0.0f,  0.0f, 0.0f,
            -1.0f,  1.0f, -1.0f, -1.0f,  0.0f,  0.0f,  0.0f, 1.0f,

             1.0f,  1.0f,  1.0f,  1.0f,  0.0f,  0.0f,  0.0f, 1.0f, // FACE DIREITA DO CUBO
             1.0f,  1.0f, -1.0f,  1.0f,  0.0f,  0.0f,  1.0f, 1.0f,
             1.0f, -1.0f, -1.0f,  1.0f,  0.0f,  0.0f,  1.0f, 0.0f,
             1.0f, -1.0f, -1.0f,  1.0f,  0.0f,  0.0f,  1.0f, 0.0f,
             1.0f, -1.0f,  1.0f,  1.0f,  0.0f,  0.0f,  0.0f, 0.0f,
             1.0f,  1.0f,  1.0f,  1.0f,  0.0f,  0.0f,  0.0f, 1.0f,

            -1.0f, -1.0f,  1.0f,  0.0f, -1.0f,  0.0f,  0.0f, 1.0f, // FACE DE BAIXO DO CUBO
             1.0f, -1.0f,  1.0f,  0.0f, -1.0f,  0.0f,  1.0f, 1.0f,
             1.0f, -1.0f, -1.0f,  0.0f, -1.0f,  0.0f,  1.0f, 0.0f,
             1.0f, -1.0f, -1.0f,  0.0f, -1.0f,  0.0f,  1.0f, 0.0f,
            -1.0f, -1.0f, -1.0f,  0.0f, -1.0f,  0.0f,  0.0f, 0.0f,
            -1.0f, -1.0f,  1.0f,  0.0f, -1.0f,  0.0f,  0.0f, 1.0f,

            -1.0f,  1.0f, -1.0f,  0.0f,  1.0f,  0.0f,  0.0f, 1.0f, // FACE DE CIMA DO CUBO
             1.0f,  1.0f, -1.0f,  0.0f,  1.0f,  0.0f,  1.0f, 1.0f,
             1.0f,  1.0f,  1.0f,  0.0f,  1.0f,  0.0f,  1.0f, 0.0f,
             1.0f,  1.0f,  1.0f,  0.0f,  1.0f,  0.0f,  1.0f, 0.0f,
            -1.0f,  1.0f,  1.0f,  0.0f,  1.0f,  0.0f,  0.0f, 0.0f,
            -1.0f,  1.0f, -1.0f,  0.0f,  1.0f,  0.0f,  0.0f, 1.0f
        };

    private readonly float[] _vertices2 =
        {
            // Positions              Texture coords
             1.0f, -1.0f, -1.0f, 0.0f, 0.0f, // FACE DE TRAS
            -1.0f, -1.0f, -1.0f, 1.0f, 0.0f,
            -1.0f,  1.0f, -1.0f, 1.0f, 1.0f,
            -1.0f,  1.0f, -1.0f, 1.0f, 1.0f,
             1.0f,  1.0f, -1.0f, 0.0f, 1.0f,
             1.0f, -1.0f, -1.0f, 0.0f, 0.0f,

            -1.0f, -1.0f,  1.0f, 0.0f, 0.0f, // FACE FRONTAL
             1.0f, -1.0f,  1.0f, 1.0f, 0.0f,
             1.0f,  1.0f,  1.0f, 1.0f, 1.0f,
             1.0f,  1.0f,  1.0f, 1.0f, 1.0f,
            -1.0f,  1.0f,  1.0f, 0.0f, 1.0f,
            -1.0f, -1.0f,  1.0f, 0.0f, 0.0f,

            -1.0f,  1.0f, -1.0f, 0.0f, 1.0f, // FACE ESQUERDA DO CUBO
            -1.0f,  1.0f,  1.0f, 1.0f, 1.0f,
            -1.0f, -1.0f,  1.0f, 1.0f, 0.0f,
            -1.0f, -1.0f,  1.0f, 1.0f, 0.0f,
            -1.0f, -1.0f, -1.0f, 0.0f, 0.0f,
            -1.0f,  1.0f, -1.0f, 0.0f, 1.0f,

             1.0f,  1.0f,  1.0f, 0.0f, 1.0f, // FACE DIREITA DO CUBO
             1.0f,  1.0f, -1.0f, 1.0f, 1.0f,
             1.0f, -1.0f, -1.0f, 1.0f, 0.0f,
             1.0f, -1.0f, -1.0f, 1.0f, 0.0f,
             1.0f, -1.0f,  1.0f, 0.0f, 0.0f,
             1.0f,  1.0f,  1.0f, 0.0f, 1.0f,

            -1.0f, -1.0f,  1.0f, 0.0f, 1.0f, // FACE DE BAIXO DO CUBO
             1.0f, -1.0f,  1.0f, 1.0f, 1.0f,
             1.0f, -1.0f, -1.0f, 1.0f, 0.0f,
             1.0f, -1.0f, -1.0f, 1.0f, 0.0f,
            -1.0f, -1.0f, -1.0f, 0.0f, 0.0f,
            -1.0f, -1.0f,  1.0f, 0.0f, 1.0f,

            -1.0f,  1.0f, -1.0f, 0.0f, 1.0f, // FACE DE CIMA DO CUBO
             1.0f,  1.0f, -1.0f, 1.0f, 1.0f,
             1.0f,  1.0f,  1.0f, 1.0f, 0.0f,
             1.0f,  1.0f,  1.0f, 1.0f, 0.0f,
            -1.0f,  1.0f,  1.0f, 0.0f, 0.0f,
            -1.0f,  1.0f, -1.0f, 0.0f, 1.0f
        };
    private readonly Vector3[] _pointLightPositions =
        {
            new Vector3(0.7f, 0.2f, 2.0f),
            new Vector3(2.3f, -3.3f, -4.0f),
            new Vector3(-4.0f, 2.0f, -12.0f),
            new Vector3(0.0f, 0.0f, -3.0f)
        };
    private readonly float[] _verticesTexture =
        {
             1.0f,  1.0f, 0.0f, 1.0f, 1.0f,
             1.0f, -1.0f, 0.0f, 1.0f, 0.0f,
            -1.0f, -1.0f, 0.0f, 0.0f, 0.0f,
            -1.0f,  1.0f, 0.0f, 0.0f, 1.0f 
        };
    private readonly uint[] _indices =
        {
            0, 1, 3,
            1, 2, 3
        };
    private Camera _camera;
    private Vector2 _lastPos;
    private float _sensitivity = 0.2f;

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
      #endregion

      _lightingMapShader = new Shader("Shaders/shaderLighting.vert", "Shaders/lighting.frag");
      _basicLightingShader = new Shader("Shaders/shaderLighting.vert", "Shaders/basic_lighting.frag");
      _lightCasterDirLightsShader = new Shader("Shaders/shaderLighting.vert", "Shaders/directional_lighting.frag");
      _lightCasterPointLightShader = new Shader("Shaders/shaderLighting.vert", "Shaders/point_lighting.frag");
      _lightCasterSpotlightShader = new Shader("Shaders/shaderLighting.vert", "Shaders/spotlight_lighting.frag");
      _multipleLightsShader = new Shader("Shaders/shaderLighting.vert", "Shaders/multiple_lighting.frag");
      
      _lampShader = new Shader("Shaders/shaderLighting.vert", "Shaders/shaderLighting.frag");
      _lightingShader = _lightingMapShader;
      
      OnLoadTextureLight();
      OnLoadTexture();

      _camera = new Camera(Vector3.UnitZ * 5, ClientSize.X / (float)ClientSize.Y);
    }

    protected override void OnRenderFrame(FrameEventArgs e)
    {
      base.OnRenderFrame(e);

      GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

      mundo.Desenhar(new Transformacao4D(), _camera);
      OnRenderTextureLight();

#if CG_Gizmo      
      Gizmo_Sru3D();
#endif
      SwapBuffers();
    }

    protected void OnRenderTextureLight()
    {
      GL.BindVertexArray(_vaoModel);

      if (_lightShaderToUse == 0)
      {
        onRenderTextureShader();
      }
      if (_lightShaderToUse == 1)
      {
        _lightingShader = _basicLightingShader;
        onRenderBasicLightingShader();
      }
      if (_lightShaderToUse == 2)
      {
        _lightingShader = _lightingMapShader;
        onRenderLightingMapShader();
      }
      if (_lightShaderToUse == 3)
      {
        _lightingShader = _lightCasterDirLightsShader;
        onRenderDirectionalLightingShader();
      }
      if (_lightShaderToUse == 4)
      {
        _lightingShader = _lightCasterPointLightShader;
        onRenderPointLightingShader();
      }
      if (_lightShaderToUse == 5)
      {
        _lightingShader = _lightCasterSpotlightShader;
        onRenderSpotlightLightingShader();
      }
      if (_lightShaderToUse == 6)
      {
        _lightingShader = _multipleLightsShader;
        onRenderMultipleLightingShader();
      }
      
    }

    protected void onRenderMultipleLightingShader()
    {
      GL.BindVertexArray(_vaoModel);

      _textureMosqueteiros.Use(TextureUnit.Texture0);;
      _lightingShader.Use();

      _lightingShader.SetMatrix4("model", Matrix4.Identity);
      _lightingShader.SetMatrix4("view", _camera.GetViewMatrix());
      _lightingShader.SetMatrix4("projection", _camera.GetProjectionMatrix());

      _lightingShader.SetVector3("viewPos", _camera.Position);

      _lightingShader.SetInt("material.diffuse", 0);
      _lightingShader.SetInt("material.specular", 1);
      _lightingShader.SetVector3("material.specular", new Vector3(0.5f, 0.5f, 0.5f));
      _lightingShader.SetFloat("material.shininess", 32.0f);

      _lightingShader.SetVector3("dirLight.direction", new Vector3(-0.2f, -1.0f, -0.3f));
      _lightingShader.SetVector3("dirLight.ambient", new Vector3(0.05f, 0.05f, 0.05f));
      _lightingShader.SetVector3("dirLight.diffuse", new Vector3(0.4f, 0.4f, 0.4f));
      _lightingShader.SetVector3("dirLight.specular", new Vector3(0.5f, 0.5f, 0.5f));

      for (int i = 0; i < _pointLightPositions.Length; i++)
      {
          _lightingShader.SetVector3($"pointLights[{i}].position", _pointLightPositions[i]);
          _lightingShader.SetVector3($"pointLights[{i}].ambient", new Vector3(0.05f, 0.05f, 0.05f));
          _lightingShader.SetVector3($"pointLights[{i}].diffuse", new Vector3(0.8f, 0.8f, 0.8f));
          _lightingShader.SetVector3($"pointLights[{i}].specular", new Vector3(1.0f, 1.0f, 1.0f));
          _lightingShader.SetFloat($"pointLights[{i}].constant", 1.0f);
          _lightingShader.SetFloat($"pointLights[{i}].linear", 0.09f);
          _lightingShader.SetFloat($"pointLights[{i}].quadratic", 0.032f);
      }

      _lightingShader.SetVector3("spotLight.position", _camera.Position);
      _lightingShader.SetVector3("spotLight.direction", _camera.Front);
      _lightingShader.SetVector3("spotLight.ambient", new Vector3(0.0f, 0.0f, 0.0f));
      _lightingShader.SetVector3("spotLight.diffuse", new Vector3(1.0f, 1.0f, 1.0f));
      _lightingShader.SetVector3("spotLight.specular", new Vector3(1.0f, 1.0f, 1.0f));
      _lightingShader.SetFloat("spotLight.constant", 1.0f);
      _lightingShader.SetFloat("spotLight.linear", 0.09f);
      _lightingShader.SetFloat("spotLight.quadratic", 0.032f);
      _lightingShader.SetFloat("spotLight.cutOff", MathF.Cos(MathHelper.DegreesToRadians(12.5f)));
      _lightingShader.SetFloat("spotLight.outerCutOff", MathF.Cos(MathHelper.DegreesToRadians(17.5f)));

      GL.DrawArrays(PrimitiveType.Triangles, 0, 36);

      GL.BindVertexArray(_vaoLamp);

      _lampShader.Use();

      _lampShader.SetMatrix4("view", _camera.GetViewMatrix());
      _lampShader.SetMatrix4("projection", _camera.GetProjectionMatrix());

      for (int i = 0; i < _pointLightPositions.Length; i++)
      {
          Matrix4 lampMatrix = Matrix4.CreateScale(0.2f);
          lampMatrix = lampMatrix * Matrix4.CreateTranslation(_pointLightPositions[i]);

          _lampShader.SetMatrix4("model", lampMatrix);

          GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
      }
    }

    protected void onRenderSpotlightLightingShader()
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

      _lightingShader.SetVector3("light.position", _camera.Position);
      _lightingShader.SetVector3("light.direction", _camera.Front);
      _lightingShader.SetFloat("light.cutOff", MathF.Cos(MathHelper.DegreesToRadians(12.5f)));
      _lightingShader.SetFloat("light.outerCutOff", MathF.Cos(MathHelper.DegreesToRadians(17.5f)));
      _lightingShader.SetFloat("light.constant", 1.0f);
      _lightingShader.SetFloat("light.linear", 0.09f);
      _lightingShader.SetFloat("light.quadratic", 0.032f);
      _lightingShader.SetVector3("light.ambient", new Vector3(0.2f));
      _lightingShader.SetVector3("light.diffuse", new Vector3(0.5f));
      _lightingShader.SetVector3("light.specular", new Vector3(1.0f));

      GL.DrawArrays(PrimitiveType.Triangles, 0, 36);

      GL.BindVertexArray(_vaoLamp);

      _lampShader.Use();

      Matrix4 lampMatrix = Matrix4.CreateScale(0.1f);
      lampMatrix = lampMatrix * Matrix4.CreateTranslation(_lightPos);

      _lampShader.SetMatrix4("model", lampMatrix);
      _lampShader.SetMatrix4("view", _camera.GetViewMatrix());
      _lampShader.SetMatrix4("projection", _camera.GetProjectionMatrix());

      GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
    }

    protected void onRenderPointLightingShader()
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
      _lightingShader.SetFloat("light.constant", 1.0f);
      _lightingShader.SetFloat("light.linear", 0.09f);
      _lightingShader.SetFloat("light.quadratic", 0.032f);
      _lightingShader.SetVector3("light.ambient", new Vector3(0.2f));
      _lightingShader.SetVector3("light.diffuse", new Vector3(0.5f));
      _lightingShader.SetVector3("light.specular", new Vector3(1.0f));

      GL.DrawArrays(PrimitiveType.Triangles, 0, 36);

      GL.BindVertexArray(_vaoLamp);

      _lampShader.Use();

      Matrix4 lampMatrix = Matrix4.CreateScale(0.1f);
      lampMatrix = lampMatrix * Matrix4.CreateTranslation(_lightPos);

      _lampShader.SetMatrix4("model", lampMatrix);
      _lampShader.SetMatrix4("view", _camera.GetViewMatrix());
      _lampShader.SetMatrix4("projection", _camera.GetProjectionMatrix());

      GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
    }

    protected void onRenderDirectionalLightingShader()
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

      _lightingShader.SetVector3("light.direction", new Vector3(-_lightPos.X, -_lightPos.Y, -_lightPos.Z));
      _lightingShader.SetVector3("light.ambient", new Vector3(0.2f));
      _lightingShader.SetVector3("light.diffuse", new Vector3(0.5f));
      _lightingShader.SetVector3("light.specular", new Vector3(1.0f));

      GL.DrawArrays(PrimitiveType.Triangles, 0, 36);

      GL.BindVertexArray(_vaoLamp);

      _lampShader.Use();

      Matrix4 lampMatrix = Matrix4.CreateScale(0.1f);
      lampMatrix = lampMatrix * Matrix4.CreateTranslation(_lightPos);

      _lampShader.SetMatrix4("model", lampMatrix);
      _lampShader.SetMatrix4("view", _camera.GetViewMatrix());
      _lampShader.SetMatrix4("projection", _camera.GetProjectionMatrix());

      GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
    }

    protected void onRenderLightingMapShader()
    {
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
      lampMatrix *= Matrix4.CreateScale(0.1f);
      lampMatrix *= Matrix4.CreateTranslation(_lightPos);

      _lampShader.SetMatrix4("model", lampMatrix);
      _lampShader.SetMatrix4("view", _camera.GetViewMatrix());
      _lampShader.SetMatrix4("projection", _camera.GetProjectionMatrix());

      GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
    }

    protected void onRenderBasicLightingShader()
    {
      GL.BindVertexArray(_vaoModel);

      _lightingShader.Use();

      _lightingShader.SetMatrix4("model", Matrix4.Identity);
      _lightingShader.SetMatrix4("view", _camera.GetViewMatrix());
      _lightingShader.SetMatrix4("projection", _camera.GetProjectionMatrix());

      _lightingShader.SetVector3("objectColor", new Vector3(1.0f, 0.5f, 0.31f));
      _lightingShader.SetVector3("lightColor", new Vector3(1.0f, 1.0f, 1.0f));
      _lightingShader.SetVector3("lightPos", _lightPos);
      _lightingShader.SetVector3("viewPos", _camera.Position);

      GL.DrawArrays(PrimitiveType.Triangles, 0, 36);

      GL.BindVertexArray(_vaoLamp);

      _lampShader.Use();

      Matrix4 lampMatrix = Matrix4.CreateScale(0.1f);
      lampMatrix = lampMatrix * Matrix4.CreateTranslation(_lightPos);

      _lampShader.SetMatrix4("model", lampMatrix);
      _lampShader.SetMatrix4("view", _camera.GetViewMatrix());
      _lampShader.SetMatrix4("projection", _camera.GetProjectionMatrix());

      GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
    }

    protected void onRenderTextureShader()
    {
      GL.BindVertexArray(_vertexArrayObject);

      _textureMosqueteiros.Use(TextureUnit.Texture0);
      _shaderTresMosqueteiros.Use();
      _shaderTresMosqueteiros.SetMatrix4("model", Matrix4.Identity);
      _shaderTresMosqueteiros.SetMatrix4("view", _camera.GetViewMatrix());
      _shaderTresMosqueteiros.SetMatrix4("projection", _camera.GetProjectionMatrix());


      GL.DrawElements(PrimitiveType.Triangles, _vertices2.Length, DrawElementsType.UnsignedInt, 0);
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
      if (estadoTeclado.IsKeyPressed(Keys.D0))
        _lightShaderToUse = 0;
      if (estadoTeclado.IsKeyPressed(Keys.D1))
        _lightShaderToUse = 1;
      if (estadoTeclado.IsKeyPressed(Keys.D2))
        _lightShaderToUse = 2;
      if (estadoTeclado.IsKeyPressed(Keys.D3))
        _lightShaderToUse = 3;
      if (estadoTeclado.IsKeyPressed(Keys.D4))
        _lightShaderToUse = 4;
      if (estadoTeclado.IsKeyPressed(Keys.D5))
        _lightShaderToUse = 5;
      if (estadoTeclado.IsKeyPressed(Keys.D6))
        _lightShaderToUse = 6;
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
      var mouse = MouseState;
      if (MouseState.IsButtonPressed(MouseButton.Right))
      {
        _lastPos = new Vector2(mouse.X, mouse.Y);
        CursorState = CursorState.Grabbed;
      }
      if (MouseState.IsButtonDown(MouseButton.Right))
      {
        float deltaX = mouse.X - _lastPos.X;
        float deltaY = mouse.Y - _lastPos.Y;
        
        _lastPos = new Vector2(mouse.X, mouse.Y);

        _camera.Yaw += deltaX * _sensitivity;
        _camera.Pitch -= deltaY * _sensitivity;
      }
      if (MouseState.IsButtonReleased(MouseButton.Right))
      {
        CursorState = CursorState.Normal;
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

      GL.DeleteProgram(_shaderBranca.Handle);
      GL.DeleteProgram(_shaderVermelha.Handle);
      GL.DeleteProgram(_shaderVerde.Handle);
      GL.DeleteProgram(_shaderAzul.Handle);
      GL.DeleteProgram(_shaderCiano.Handle);
      GL.DeleteProgram(_shaderMagenta.Handle);
      GL.DeleteProgram(_shaderAmarela.Handle);
      GL.DeleteProgram(_shaderTresMosqueteiros.Handle);
      GL.DeleteProgram(_lightingShader.Handle);
      GL.DeleteProgram(_lampShader.Handle);


      base.OnUnload();
    }

    protected void OnLoadTexture()
    {
      _vertexArrayObject = GL.GenVertexArray();
      GL.BindVertexArray(_vertexArrayObject);

      _vertexBufferObject = GL.GenBuffer();
      GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
      GL.BufferData(BufferTarget.ArrayBuffer, _vertices2.Length * sizeof(float), _vertices2, BufferUsageHint.StaticDraw);

      _elementBufferObject = GL.GenBuffer();
      GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);
      GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(uint), _indices, BufferUsageHint.StaticDraw);

      _shaderTresMosqueteiros.Use();

      var vertexLocation = _shaderTresMosqueteiros.GetAttribLocation("aPosition");
      GL.EnableVertexAttribArray(vertexLocation);
      GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);

      var texCoordLocation = _shaderTresMosqueteiros.GetAttribLocation("aTexCoord");
      GL.EnableVertexAttribArray(texCoordLocation);
      GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));

      _textureMosqueteiros.Use(TextureUnit.Texture0);
    }

    protected void OnLoadTextureLight()
    {
      _vertexBufferObject_light = GL.GenBuffer();
      GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject_light);
      GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.StaticDraw);

      {
        _vaoModel = GL.GenVertexArray();
        GL.BindVertexArray(_vaoModel);

        var positionLocation = _lightingShader.GetAttribLocation("aPos");
        GL.EnableVertexAttribArray(positionLocation);
        GL.VertexAttribPointer(positionLocation, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 0);

        var normalLocation = _lightingShader.GetAttribLocation("aNormal");
        GL.EnableVertexAttribArray(normalLocation);
        GL.VertexAttribPointer(normalLocation, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 3 * sizeof(float));

        var texCoordLocation = _lightingShader.GetAttribLocation("aTexCoords");
        GL.EnableVertexAttribArray(texCoordLocation);
        GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 8 * sizeof(float), 6 * sizeof(float));
      }
            
      {
          _vaoLamp = GL.GenVertexArray();
          GL.BindVertexArray(_vaoLamp);

          var positionLocation = _lampShader.GetAttribLocation("aPos");
          GL.EnableVertexAttribArray(positionLocation);
          GL.VertexAttribPointer(positionLocation, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 0);
      }

      _textureMosqueteiros = Texture.LoadFromFile("Textures/tres_mosqueteiros.png");
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
