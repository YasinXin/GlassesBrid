using UnityEngine;

public static class MatrixTool
{
    /// 矩阵格式：
    /// m00,m01,m02,m03
    /// m10,m11,m12,m13
    /// m20,m21,m22,m23
    /// m30,m31,m32,m33
    /// 缩放：m00,m11,m22
    /// 位置：m03,m13,m23
    /// 旋转X：m11,m22,m21,m22
    /// 旋转Y：m00,m02,m20,m22
    /// 旋转Z：m00,m01,m10,m11

    public static Quaternion GetRotation(Matrix4x4 matrix)
    {
    	Matrix4x4 m4=Matrix4x4.identity;
        //Vector3 vScale=GetScale(matrix);
        float x = Mathf.Sqrt(matrix.m00 * matrix.m00 + matrix.m01 * matrix.m01 + matrix.m02 * matrix.m02);
        float y = Mathf.Sqrt(matrix.m10 * matrix.m10 + matrix.m11 * matrix.m11 + matrix.m12 * matrix.m12);
        float z = Mathf.Sqrt(matrix.m20 * matrix.m20 + matrix.m21 * matrix.m21 + matrix.m22 * matrix.m22);
        //Debug.Log ("===Quaternion  GetScale  ===>"+x+","+y+","+z);
        // Vector3  v3= new Vector3(x, y,z); 
        x=1/x;
        y=1/y;
        z=1/z;

    	m4.m00 = matrix.m00 * x;
        m4.m01 = matrix.m01 * y;
        m4.m02 = matrix.m02 * z;
        m4.m10 = matrix.m10 * x;
        m4.m11 = matrix.m11 * y;
        m4.m12 = matrix.m12 * z;
        m4.m20 = matrix.m20 * x;
        m4.m21 = matrix.m21 * y;
        m4.m22 = matrix.m22 * z;

		// float num = m4.m00 + m4.m11 + m4.m22;
		// Debug.Log("GetRotation===>num  ="+num);
  //       // to quaternion
  //       float qw = -(Mathf.Sqrt(1.0f + num)*0.5f);
  //       Debug.Log ("===Quaternion   qw  ===>"+qw);
  //       float w = 4.0f * (qw);
  //       float qx = (m4.m21 - m4.m12) / w;
  //       float qy = (m4.m02 - m4.m20) / w;
  //       float qz = (m4.m10 - m4.m01) / w;

		// float v=qx*qx+qy*qy+qz*qz+qw*qw;

		// Debug.Log ("===Quater  ===>"+qx.ToString()+","+qy.ToString()+","+qz.ToString()+","+qw.ToString()+"======== "+v);

 		// return new Quaternion(qx,qy, qz, qw);

		   
   		return QuaternionFromMatrix(m4);
    }

	public static Quaternion QuaternionFromMatrix(Matrix4x4 m) {
		Quaternion q = new Quaternion();
		q.w = Mathf.Sqrt( Mathf.Max( 0, 1 + m[0,0] + m[1,1] + m[2,2] ) ) *0.5f; 
		q.x = Mathf.Sqrt( Mathf.Max( 0, 1 + m[0,0] - m[1,1] - m[2,2] ) ) *0.5f; 
		q.y = Mathf.Sqrt( Mathf.Max( 0, 1 - m[0,0] + m[1,1] - m[2,2] ) ) *0.5f; 
		q.z = Mathf.Sqrt( Mathf.Max( 0, 1 - m[0,0] - m[1,1] + m[2,2] ) ) *0.5f; 
		q.x *= Mathf.Sign( q.x * ( m[2,1] - m[1,2] ) );
		q.y *= Mathf.Sign( q.y * ( m[0,2] - m[2,0] ) );
		q.z *= Mathf.Sign( q.z * ( m[1,0] - m[0,1] ) );

		return q;
	}

	public static Vector3 GetRotationEulerAngles(Matrix4x4 matrix)
	{
		Matrix4x4 m4=Matrix4x4.identity;
        float s_x = Mathf.Sqrt(matrix.m00 * matrix.m00 + matrix.m01 * matrix.m01 + matrix.m02 * matrix.m02);
        float s_y = Mathf.Sqrt(matrix.m10 * matrix.m10 + matrix.m11 * matrix.m11 + matrix.m12 * matrix.m12);
        float s_z = Mathf.Sqrt(matrix.m20 * matrix.m20 + matrix.m21 * matrix.m21 + matrix.m22 * matrix.m22);
        
        s_x=1/s_x;
        s_y=1/s_y;
        s_z=1/s_z;

    	m4.m00 = matrix.m00 * s_x;
        m4.m01 = matrix.m01 * s_y;
        m4.m02 = matrix.m02 * s_z;
        m4.m10 = matrix.m10 * s_x;
        m4.m11 = matrix.m11 * s_y;
        m4.m12 = matrix.m12 * s_z;
        m4.m20 = matrix.m20 * s_x;
        m4.m21 = matrix.m21 * s_y;
        m4.m22 = matrix.m22 * s_z;

    //     m4.m00=0.877686f;	
  		// m4.m01=0.397560f;	
    // 	m4.m02=-0.267605f;	
    //   	m4.m10=-0.414454f;	
    //     m4.m11=0.910041f;	
    //     m4.m12=-0.007341f;	
    //     m4.m20=0.240613f;	
    //     m4.m21=0.117353f;	
    //     m4.m22=0.963501f;	

    //     m4.m00=1.0f;	
  		// m4.m01=0.0f;	
    // 	m4.m02=0.0f;	
    //   	m4.m10=0.0f;	
    //     m4.m11=1.0f;	
    //     m4.m12=0.0f;	
    //     m4.m20=0.0f;	
    //     m4.m21=0.0f;	
    //     m4.m22=1.0f;	


		float x = Mathf.Atan2 (m4.m21, m4.m22);
		float y = Mathf.Atan2 (-m4.m20,Mathf.Sqrt(m4.m21*m4.m21+m4.m22*m4.m22));
		float z = Mathf.Atan2 (m4.m10, m4.m00);
		float r_x = x * 180.0f / Mathf.PI;
		float r_y = y * 180.0f / Mathf.PI;
		float r_z = z * 180.0f / Mathf.PI;

		return new Vector3(r_x,r_y,r_z);
	}

    public static Vector3 GetPosition(Matrix4x4 matrix)
    {
        float x =matrix.m03;
        float y =matrix.m13;
        float z =matrix.m23;
        return new Vector3(x, y,z);
    }

    public static Vector3 GetScale(Matrix4x4 matrix)
    {
        float x = Mathf.Sqrt(matrix.m00 * matrix.m00 + matrix.m01 * matrix.m01 + matrix.m02 * matrix.m02);
        float y = Mathf.Sqrt(matrix.m10 * matrix.m10 + matrix.m11 * matrix.m11 + matrix.m12 * matrix.m12);
        float z = Mathf.Sqrt(matrix.m20 * matrix.m20 + matrix.m21 * matrix.m21 + matrix.m22 * matrix.m22);
        return new Vector3(x, y,z);
    }
}
