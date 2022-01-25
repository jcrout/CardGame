Project requirements are shown in the Project Requirements.docx file.



This project is over-engineered to a large extent. This much is very apparent with just a quick glance at the code, spread over several project files with configuration files too. However, this project was to be treated as a professional project with extensible/reusable code, and thus I settled on these different patterns and principals:

-SOLID Principles
-Dependency Injection, with only constructor injection used
--IoC container to wire up objects with minimal usage of the 'new' keyword outside of factories
-Abstract Factories
-CQRS (with the CardGame and its command handler classes)
-Configuration files, to allow changing various settings such as min/max player count and penalty card count without having to recompile the program.

Because most of the core functionality is handled in classes with a single responsibility, aspects such as the scoring/deck+shuffling/etc. can be re-used for other card-based games. Not only that, but even the most basic components such as the card itself along with its suit and face value can be extended to contain more data whilst still working with other components such as the IDeck implementations taking in ICards. A graphical event-driven application could be created while re-using many of these components as well. Additionally, a non-Console text-based implementation could also be created without having to change much of the code (mostly, just implement ITextInterface and change the composition root).

Aspects like enforcing the Guard class usage to protect against nulls were enforced using Roslyn code analyzers and automatic code fixers, which made it easy to enforce consistency in their usage.
