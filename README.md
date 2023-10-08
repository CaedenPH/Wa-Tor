## Wa-Tor Algorithm

[c-sharp-wa-tor.webm](https://github.com/CaedenPH/Wa-Tor/assets/23421705/5446cfc9-c48d-450c-9325-138b09acbce1)


Wa-Tor is a population dynamics simulation devised by A. K. Dewdney. It is usually implemented as a two-dimensional grid with three colours, one for fish, one for sharks and one for empty water. The sharks are predatory and eat the fish. Both sharks and fish live, move, reproduce and die in Wa-Tor according to the simple rules defined below. From these simple rules, complex emergent behavior can be seen to arise.

### Algorithm Overview
In the Wa-Tor universe, a grid represents an ocean where fish and sharks coexist, each following specific rules that govern their movement, reproduction, and survival:

**Fish:**
- Move: Randomly to an adjacent square if available.
- Reproduce: After a set number of chronons (time steps), if moving to a new square.
**Sharks:**
- Move: Preferentially to an adjacent square containing fish, consuming them; otherwise, move randomly.
- Reproduce: Similar to fish, but with a different reproduction interval.
- Starve: If a shark does not eat within a certain number of chronons, it dies.

### Implementation in C#
This pure C# implemnetation implements the rules of the Wa-Tor algorithm while maximising the randomness by randomising directional movements (denoted in the program as compass directions), entity selection, and entity creation.

**Key Features:**
- Random Entity Placement: Initial placement of fish and sharks is randomized to ensure varied starting conditions.
- Balanced Ecosystem: Introduces mechanisms to balance the number of predators and prey, preventing unchecked domination by either.
- Visual Display: Each state of the grid is optionally displayed in the console, providing a visual representation of the ecosystem over time.

### Testing
All classes and methods are tested using the MSTest Framework.
In the future a code coverage tool could be implemented to see what percent of the whole project is tested versus untested.

Automated tests are also ran through github CI workflows.

#### References:
- [Wikipedia - Wa-Tor](https://en.wikipedia.org/wiki/Wa-Tor)
- [Beltoforion - Detailed Wa-Tor Explanation](https://beltoforion.de/en/wator)
