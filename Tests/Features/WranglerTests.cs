using LightBDD.Framework.Scenarios;
using LightBDD.MsTest3;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Features;

[TestClass]
public partial class WranglerTests : BaseTestFrame
{
    [Scenario]
    [TestMethod]
    public async Task Happy_path()
    {
        await Runner.RunScenarioAsync(
            _ => Setup_scenarios(),
            _ => Planes_comes_in(1),
            _ => Planes_comes_in(2),
            _ => Compile_for_time(4),
            _ => Get_planes(),
            _ => The_right_planes_are_there()
                );
    }


}
