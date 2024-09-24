using System.Globalization;
using CG_Biblioteca;
using gcgcg;
using OpenTK.Graphics.OpenGL4;

internal class SrPalito : Objeto
{
    private Ponto4D startPoint;
    private Ponto4D endPoint;
    private double radius;
    private double angle;

    public SrPalito(Objeto _paiRef, ref char _rotulo, Objeto objetoFilho = null) : base(_paiRef, ref _rotulo, objetoFilho)
    {
    }

    public SrPalito(Objeto _paiRef, ref char _rotulo) : base(_paiRef, ref _rotulo)
    {
        PrimitivaTipo = PrimitiveType.Lines;
        PrimitivaTamanho = 5;
        angle = 45;
        radius = 0.5f;
        startPoint = new Ponto4D(0,0);
        endPoint = Matematica.GerarPtosCirculo(angle, radius);
        Atualizar();
    }

    public void Atualizar()
    {
      PontosApagar();
      PontosAdicionar(startPoint);
      PontosAdicionar(endPoint);
    }

    public void AtualizarPe(double peInc)
    {
        startPoint.X += peInc;
        endPoint.X += peInc;
        Atualizar();
    }

    public void AtualizarRaio(double raioInc) {
        radius += raioInc;
        Ponto4D circlePoint = Matematica.GerarPtosCirculo(angle, radius);
        endPoint.X = circlePoint.X + startPoint.X;
        endPoint.Y = circlePoint.Y;
        Atualizar();
    }

    public void AtualizarAngulo(double anguloInc)
    {
        angle = (angle + anguloInc) % 360;
        Ponto4D circlePoint = Matematica.GerarPtosCirculo(angle, radius);
        endPoint.X = circlePoint.X + startPoint.X;
        endPoint.Y = circlePoint.Y;
        Atualizar();
    }

    public override string ToString()
    {
      string retorno;
      retorno = "__ Objeto Circulo _ Tipo: " + PrimitivaTipo + " _ Tamanho: " + PrimitivaTamanho + "\n";
      retorno += base.ImprimeToString();
      return retorno;
    }
}