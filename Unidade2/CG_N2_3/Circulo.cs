using System.Globalization;
using CG_Biblioteca;
using gcgcg;
using OpenTK.Graphics.OpenGL4;

internal class Circulo : Objeto
{
    private const int numberOfPoints = 72;
    private Ponto4D[] generatedCiclePoints = new Ponto4D[numberOfPoints];
    public Circulo(Objeto _paiRef, ref char _rotulo, Objeto objetoFilho = null) : base(_paiRef, ref _rotulo, objetoFilho)
    {
    }

    public Circulo(Objeto _paiRef, ref char _rotulo, double _raio, Ponto4D ptoDeslocamento) : base(_paiRef, ref _rotulo)
    {
      PrimitivaTipo = PrimitiveType.Points;
      PrimitivaTamanho = 5;
      double increaseAngle = 360/numberOfPoints;
      double drawingAngle = increaseAngle;
      for (int i = 0; i < numberOfPoints; i++) {
        Ponto4D ptoCircle = Matematica.GerarPtosCirculo(drawingAngle, _raio);
        generatedCiclePoints[i] = new Ponto4D(ptoCircle.X, ptoCircle.Y);
        ptoCircle.X += ptoDeslocamento.X;
        ptoCircle.Y += ptoDeslocamento.Y;
        PontosAdicionar(ptoCircle);
        drawingAngle += increaseAngle;
      }
    }

    public void Atualizar(Ponto4D ptoDeslocamento)
    {
      PontosApagar();
      double increaseAngle = 360/numberOfPoints;
      double drawingAngle = increaseAngle;
      for (int i = 0; i < numberOfPoints; i++) {
        Ponto4D ptoCircle = generatedCiclePoints[i];
        ptoCircle.X += ptoDeslocamento.X;
        ptoCircle.Y += ptoDeslocamento.Y;
        PontosAdicionar(ptoCircle);
        drawingAngle += increaseAngle;
      }
    }

#if CG_Debug
    public override string ToString()
    {
      string retorno;
      retorno = "__ Objeto Circulo _ Tipo: " + PrimitivaTipo + " _ Tamanho: " + PrimitivaTamanho + "\n";
      retorno += base.ImprimeToString();
      return retorno;
    }
#endif
}