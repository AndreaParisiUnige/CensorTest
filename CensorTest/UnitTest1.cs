using Censor;
using Moq;
using NUnit.Framework;

namespace CensorTest {

    [TestFixture]
    public class CensorTests {

        [TestCase ()]
        [TestCase ("pippo")]
        [TestCase ("pippo", "pluto")]

        public void NothingToFilter(params string[] theMsgSequence) {
            var theSequence = new Mock<I>[theMsgSequence.Length]; // Costruisco un array di Mock di lunghezza pari al numero di stringhe per non dover istanziare I
            for (int i = 0; i < theMsgSequence.Length; i++) {
                theSequence[i] = new Mock<I>();
                theSequence[i].Setup(x => x.Message).Returns(theMsgSequence[i]);
            }

            var enumerable = theSequence.Select(m => m.Object);
            var result = CensorClass.Censor(theSequence.Select(m => m.Object), "kkzt");
            Assert.That(result, Is.EqualTo(enumerable));
        }

        [TestCase (10)]
        public void NothingToFilterInfiniteApprox(int approx) {
            var theSequence = CreateFiniteSequence(approx);

            IEnumerable<I> Infinite() {
                for (int i = 0; i < approx; i++)
                    yield return theSequence[i];
                while (true) {
                    var a = new Mock<I>();
                    a.Setup(x => x.Message).Returns("puffo");
                    yield return a.Object;
                }
            } // Uso Take per limitare il numero di elementi della sequenza infinita ad approx
            var result = CensorClass.Censor(Infinite(), "kkzt").Take(approx); 
            Assert.That(result, Is.EqualTo(theSequence));
        }

        private static I[] CreateFiniteSequence(int approx) {
            var theSequence = new I[approx];    // Crea un array di oggetti di tipo I, al cui interno inserirà i Mock
            for (int i = 0; i < approx; i++) {
                var a = new Mock<I>();
                a.Setup(x => x.Message).Returns("puffo");
                theSequence[i] = a.Object;
            }
            return theSequence;
        }

        private static I[] CreateFiniteSequence(string[] messages) {
            var theSequence = new I[messages.Length];
            for (int i = 0; i < messages.Length; i++) {
                var a = new Mock<I>();
                a.Setup(x => x.Message).Returns(messages[i]);
                theSequence[i] = a.Object;
            }
            return theSequence;
        }

        [Test]
        public void OneMatchInTheMiddle() {
            var badWord = "fuck";
            var mock = new Mock<I>();
            mock.Setup(x => x.Message).Returns($"adadada {badWord}asdadada");
            var result = CensorClass.Censor(new[] { mock.Object }, badWord);
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void FilterManyElements() {
            var badWord = "fuck";
            var inputSequenceMsg = new[] { "puffo", "topolino", $"prima{badWord}dopo", "paperino", $"{badWord}", $"{badWord} in testa" };
            var myInput = CreateFiniteSequence(inputSequenceMsg);
            var expected = new I[3];
            expected[0] = myInput[0];
            expected[1] = myInput[1];
            expected[2] = myInput[3];
            var result = CensorClass.Censor(myInput, badWord);
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void OnNullSequenceThrows() {
            Assert.That(() => CensorClass.Censor(null, "puffo"), Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void OnSequenceWithNullThrows() {
            var myInput = CreateFiniteSequence(5);
            myInput[2] = null;  // Eseguo Censor sui primi 3 elementi, mediante Count itero in modo completo su questi 3 scatenando l'eccezione
            Assert.That(() => CensorClass.Censor(myInput, "puffo").Take(3).Count(), Throws.TypeOf<ArgumentNullException>());
        }
    }
}