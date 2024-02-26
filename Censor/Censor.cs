using System.Text.RegularExpressions;

namespace Censor {

    public interface I {
        string Message { get; }
    }

    public static class CensorClass {
        public static IEnumerable<I> Censor(IEnumerable<I> sequence, string badWord) {
            if (null == sequence)
                throw new ArgumentNullException(nameof(sequence), "cannot be null");
            return SafeCensor();

            IEnumerable<I> SafeCensor() {
                foreach (var x in sequence) {
                    if (null == x)
                        throw new ArgumentNullException(nameof(sequence), "cannot contain a null element");
                    if (!new Regex(badWord).IsMatch(x.Message))   // Se c'è la parola la salto e vado oltre
                        yield return x;
                }
            }
        }
    }
}
