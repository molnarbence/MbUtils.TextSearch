using BenchmarkDotNet.Attributes;
using Core.Strategies;

namespace Benchmarks;

public class SearchComparison
{
    private const string Pattern = "target";
    private readonly StringSplitStrategy _stringSplitStrategy = new();
    private readonly KnuthMorrisPratt _knuthMorrisPratt = new();
    private readonly RegexStrategy _regexStrategy = new();

    private string _input = string.Empty;
    
    [GlobalSetup]
    public void GlobalSetup()
    {
        // Setup
        _input = """
                 Lorem ipsum dolor, sit amet consectetur adipisicing elit. 
                 Officiis vitae, odio vero target quia quae dolore, sit esse corrupti 
                 quisquam dolores in quis consectetur molestias possimus minima magnam corporis, illo quod?
                 Lorem ipsum dolor sit amet consectetur target adipisicing elit. Voluptatum alias necessitatibus aspernatur 
                 nesciunt, molestias a itaque, sapiente sit totam atque eligendi non beatae, vero est nobis? 
                 Vero accusamus eaque optio?
                 Lorem ipsum dolor, sit amet consectetur adipisicing elit. 
                 Officiis vitae, odio vero quia quae dolore, sit esse corrupti 
                 quisquam dolores in quis consectetur molestias possimus minima magnam corporis, illo quod?
                 Lorem ipsum dolor sit amet consectetur adipisicing target elit. Voluptatum alias necessitatibus aspernatur 
                 nesciunt, molestias a itaque, sapiente sit totam atque eligendi non beatae, vero est nobis? 
                 Vero accusamus eaque optio?
                 Lorem ipsum dolor, sit amet consectetur adipisicing elit. 
                 Officiis vitae, odio vero quia quae dolore, sit esse corrupti 
                 quisquam dolores in quis target consectetur molestias possimus minima magnam corporis, illo quod?
                 Lorem ipsum dolor sit amet consectetur adipisicing elit. Voluptatum alias necessitatibus aspernatur 
                 nesciunt, molestias a itaque, sapiente sit totam atque eligendi non beatae, vero est nobis? 
                 Vero accusamus eaque optio?
                 """;
    }

    [Benchmark(Baseline = true)]
    public int StringSplit() => _stringSplitStrategy.Count(_input, Pattern);

    [Benchmark]
    public int KnuthMorrisPratt() => _knuthMorrisPratt.Count(_input, Pattern);

    [Benchmark]
    public int Regex() => _regexStrategy.Count(_input, Pattern);
}