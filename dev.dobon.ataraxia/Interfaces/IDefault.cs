namespace dev.dobon.ataraxia.Interfaces;

public interface IDefault<out T>
{
    public static abstract T Default();
};