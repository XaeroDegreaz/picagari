using System;

namespace Picagari.Exceptions
{
    /// <summary>
    ///     Produces good information about problems when injecting members.
    /// </summary>
    public class PicagariException : Exception
    {
        public const string CannotUseMethodAsPostConstructDelegate =
        "Cannot use method {0}.{1} as a PostConstructAttribute method. It should return void, and have no parameters.";

        public const string InjectWillCauseInfiniteRecursion =
        "Injecting the member {0} inside {1} would cause infinite recursion.";

        public const string CannotConstructInterfaceByItself =
        "You should create a producer for, or have at least one class that implements {0}.";

        public const string NeedsParameterlessConstructor =
        "Picagari requires a parameterless constructor in order to inject {0}.";

        public const string UnknownInjectionError =
        "Picagari somehow came up injecting a null value for {0}. Is there a producer for this type?";

        public const string NoDefaultNoProducer =
        "More than one implementation exists for {0}. There are no producers, and no default has been specified for injection.";

        public const string TooManyProducersForType =
        "There can only be one producer for {0}, and there is already one at {1}.{2}";

        public PicagariException( string message ) : base( message ) {}

        public PicagariException( string message, Exception e ) : base( message, e ) {}

        public PicagariException( string message, object[] args ) : base( string.Format( message, args ) ) {}

        public PicagariException( string message, object[] args, Exception e ) : base( string.Format( message, args ), e ) {}
    }
}
