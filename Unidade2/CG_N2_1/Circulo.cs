using CG_Biblioteca;
using OpenTK.Graphics.OpenGL4;

namespace gcgcg
{
    public class Circulo : Objeto
    {
        public Circulo(Objeto _paiRef, ref char _rotulo) : this(_paiRef, ref _rotulo, 0.5, new Ponto4D())
        {

        }

        public Circulo(Objeto _paiRef, ref char _rotulo, double _raio, Ponto4D ptoDeslocamento) : base(_paiRef, ref _rotulo)
        {
            PrimitivaTipo = PrimitiveType.Points;
            PrimitivaTamanho = 5;
            Atualizar(ptoDeslocamento);
            int pontos = 72;
            // como sao 72 pontos 360 / 72 = 5
            double anguloCadaPonto = 5.0;
            // comeca com 1 pois o primeiro ponto, no angulo 0, ja esta cadastrados
            for (int i = 1; i < pontos; i++)
            {
                this.Atualizar(Matematica.GerarPtosCirculo(i * anguloCadaPonto, _raio));
            }
        }
        public void Atualizar(Ponto4D ptoDeslocamento)
        {
            base.PontosAdicionar(ptoDeslocamento);
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
}