using System.Globalization;
using CG_Biblioteca;
using OpenTK.Graphics.OpenGL4;

internal class Circulo : Objeto
{
    private const int numberOfPoints = 72;
    public Circulo(Objeto _paiRef, ref char _rotulo, Objeto objetoFilho = null) : base(_paiRef, ref _rotulo, objetoFilho)
    {
    }

    public Circulo(Objeto _paiRef, ref char _rotulo, double _raio) : base(_paiRef, ref _rotulo)
    {
      PrimitivaTipo = PrimitiveType.Points;
      PrimitivaTamanho = 5;
      double increaseAngle = 360/numberOfPoints;
      double drawingAngle = increaseAngle;
      for (int i = 0; i < numberOfPoints; i++) {
        PontosAdicionar(Matematica.GerarPtosCirculo(drawingAngle, _raio));
        drawingAngle += increaseAngle;
      }
    }

    private void Atualizar()
    {

      base.ObjetoAtualizar();
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