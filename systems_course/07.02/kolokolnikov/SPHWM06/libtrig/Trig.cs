namespace libtrig;

public static class Trig {
    public static double AreaSquare(double side) {
        return side * side;
    }
    public static double AreaRect(double width, double height) {
        return width * height;
    }
    public static double AreaTriangle(double @base, double height) {
        return 0.5 * @base * height;
    }
}
