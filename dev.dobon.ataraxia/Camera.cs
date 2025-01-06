using Microsoft.Xna.Framework;

namespace dev.dobon.ataraxia;

public sealed class Camera
{
    private Matrix _matrix = Matrix.CreateTranslation(0,0,0);

    private Vector2 _position = Vector2.Zero;

    private float _zoom = 1.0f;
    
    // Allows us to get the Matrix
    public Matrix getMatrix() {
        return _matrix;
    }

    // Private function used to refresh the matrix calculations
    // whenever the position or scale is changed
    private void setMatrix() {
        this._matrix = Matrix.CreateTranslation(this._position.X, this._position.Y, 0) * Matrix.CreateScale(new Vector3(this._zoom, this._zoom, 1));
    }
    
    // Pass in an X and or Y value to be added to the Matrix current position
    public void adjustPosition(float x, float y) {
        this._position = this._position + new Vector2(x , y);
        this.setMatrix();
    }

    // Pass in a zoom value in order to alter the current zoom level
    public void adjustZoom(float zoom)
    {
        this._zoom = this._zoom + zoom;
        this.setMatrix();
    }
}