# Third-party routines

The Steel Mill (`CPack_SteelMill.lsp`), Fastener Bin (`CPack_Fasteners.lsp`) and Weld Symbol
(`CPack_Weld.lsp`) routines, with their dialogs, slides and data tables in `support/`, are older
AutoLISP programs (STL, FAST and WELD.LSP) that CadPack bundles and launches
from its ribbon. Their original authors are not named in the files. `support/Stl_Help.txt` credits
the metric shape tables to BEAM.LSP by Steve Q. Stapleton and Bill Le Couteur.

CadPack renamed their commands, fixed their error handling and wrapped each command in one undo
group. The repository license covers CadPack's own code; these files carry no licence statement, so no terms are granted for them here.
