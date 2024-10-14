using System.Collections.Generic;
using CG_Biblioteca;
using OpenTK.Graphics.OpenGL4;

namespace gcgcg
{
  internal class Spline : Objeto
  {
    private int qtdPontos = 6;
    private List<Ponto4D> interpolation_points = new List<Ponto4D>();
    public Spline(Objeto _paiRef, ref char _rotulo) : base(_paiRef, ref _rotulo)
    {
        PrimitivaTipo = PrimitiveType.LineStrip;
    }

    public void SplineQtdPto(int inc)
    {
        if (qtdPontos + inc == 1) {
            return;
        }
        qtdPontos += inc;
        Atualizar();
    }

    public void AtualizarSpline(Ponto4D ptoInc, bool proximo)
    {
        interpolation_points.Add(ptoInc);
        if (proximo) return;
        Atualizar();
    }

    public void Atualizar()
    {
        base.PontosApagar();
        for (double i = 0; i <= qtdPontos; i++)
        {
            double t = i/qtdPontos;
            Ponto4D P0P1 = Matematica.InterpolarRetaPonto(interpolation_points[0], interpolation_points[1], t);
            Ponto4D P1P2 = Matematica.InterpolarRetaPonto(interpolation_points[1], interpolation_points[2], t);
            Ponto4D P2P3 = Matematica.InterpolarRetaPonto(interpolation_points[2], interpolation_points[3], t);

            Ponto4D P0P1P2 = Matematica.InterpolarRetaPonto(P0P1, P1P2, t);
            Ponto4D P1P2P3 = Matematica.InterpolarRetaPonto(P1P2, P2P3, t);

            Ponto4D P0P1P2P3 = Matematica.InterpolarRetaPonto(P0P1P2, P1P2P3, t);
            PontosAdicionar(P0P1P2P3);
        }
        base.ObjetoAtualizar();
    }

#if CG_Debug
    public override string ToString()
    {
      string retorno;
      retorno = "__ Objeto Ponto _ Tipo: " + PrimitivaTipo + " _ Tamanho: " + PrimitivaTamanho + "\n";
      retorno += base.ImprimeToString();
      return retorno;
    }
#endif

  }
}
