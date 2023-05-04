using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACEOCustomBuildables
{
    public interface IBuildableSourceCreator
    {
        List<TexturedBuildableMod> buildableMods { get; set; }

        List<string> modPaths { get; set; }

        void SetUp();

        void ClearBuildableMods(bool clearAllCreators);

        void ImportMods();
    }
}
