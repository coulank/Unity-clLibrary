using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace clController
{
    /// <summary>
    /// ベクトル補正クラス
    /// </summary>
    public class VecComp
    {
        /// <summary>時計回りかどうか、yを自動的に負に変換します（その必要がなければfalseにしてください）</summary>
        const bool CLOCKWISE = true;
        /// <summary>弧度法のときに負の範囲を含めるか(-180～180)、そうでなければ360までの範囲となる</summary>
        const bool DEGMINUS = false;
        /// <summary>
        /// 絶対値を使用した最大の長さのベクトルを求める
        /// </summary>
        public static Vector4 AbsMax(Vector4 lhs, Vector4 rhs)
        {
            if (rhs != Vector4.zero)
            {
                if (Mathf.Abs(rhs.x) > Mathf.Abs(lhs.x)) lhs.x = rhs.x;
                if (Mathf.Abs(rhs.y) > Mathf.Abs(lhs.y)) lhs.y = rhs.y;
                if (Mathf.Abs(rhs.z) > Mathf.Abs(lhs.z)) lhs.z = rhs.z;
                if (Mathf.Abs(rhs.w) > Mathf.Abs(lhs.w)) lhs.w = rhs.w;
            }
            return lhs;
        }
        /// <summary>
        /// 絶対値を使用した最小の長さのベクトルを求める
        /// </summary>
        public static Vector4 AbsMin(Vector4 lhs, Vector4 rhs)
        {
            if (lhs != Vector4.zero)
            {
                if (Mathf.Abs(rhs.x) < Mathf.Abs(lhs.x)) lhs.x = rhs.x;
                if (Mathf.Abs(rhs.y) < Mathf.Abs(lhs.y)) lhs.y = rhs.y;
                if (Mathf.Abs(rhs.z) < Mathf.Abs(lhs.z)) lhs.z = rhs.z;
                if (Mathf.Abs(rhs.w) < Mathf.Abs(lhs.w)) lhs.w = rhs.w;
            }
            return lhs;
        }
        /// <summary>
        /// 絶対値を使用した最大の長さのベクトルを求める
        /// オーバーヘッド処理を省いたもの
        /// </summary>
        public static Vector2 AbsMax2(Vector2 lhs, Vector2 rhs)
        {
            if (rhs != Vector2.zero)
            {
                if (Mathf.Abs(rhs.x) > Mathf.Abs(lhs.x)) lhs.x = rhs.x;
                if (Mathf.Abs(rhs.y) > Mathf.Abs(lhs.y)) lhs.y = rhs.y;
            }
            return lhs;
        }
        /// <summary>
        /// １つのベクトルから各要素のうちの最大の長さを求める
        /// </summary>
        public static float MaxRadius(Vector4 vec)
        {
            float mv = Mathf.Abs(vec.x), vl;
            if ((vl = Mathf.Abs(vec.y)) > mv) mv = vl;
            if ((vl = Mathf.Abs(vec.z)) > mv) mv = vl;
            if ((vl = Mathf.Abs(vec.w)) > mv) mv = vl;
            return mv;
        }
        /// <summary>
        /// 絶対値によって比較切り捨て、デフォでe-7よりも小さければ0を返す関数
        /// </summary>
        public static float AbsCompFloor(float val, float minpow = -7)
        {
            if (Mathf.Abs(val) < Mathf.Pow(10, minpow)) val = 0f;
            return val;
        }

        /// <summary>
        /// 絶対値によって比較切り捨て、デフォでe-4よりも小さければ0を返す関数
        /// </summary>
        public static Vector3 AbsCompFloor(Vector3 vec, float minpow = -4)
        {
            vec.x = AbsCompFloor(vec.x, minpow);
            vec.y = AbsCompFloor(vec.y, minpow);
            vec.z = AbsCompFloor(vec.z, minpow);
            return vec;
        }

        /// <summary>
        /// Atan2の補正関数、デフォルトで時計回りで出力する
        /// </summary>
        /// <param name="clockwise">時計回りかどうか</param>
        public static float Atan2c(float y, float x, bool clockwise = CLOCKWISE)
        {
            if (clockwise) y = -y;
            return Mathf.Atan2(clockwise ? -y : y, x);
        }
        /// <summary>
        /// Sinの補正関数、デフォルトで時計回りで出力する
        /// </summary>
        /// <param name="clockwise">時計回りかどうか</param>
        public static float Sin2c(float f, bool clockwise = CLOCKWISE)
        {
            return Mathf.Sin(clockwise ? -f : f);
        }

        /// <summary>
        /// 最大半径から0～conv_radiusの範囲のベクトルに変換、upper_limitは超えた場合は上限値にするかのフラグ
        /// </summary>
        public static Vector2 ConvMag(Vector2 vec, float max_radius = 1f, float conv_radius = 1f, bool upper_limit = true)
        {
            if ((vec != Vector2.zero) && (max_radius > 0f))
            {
                float radius = vec.magnitude;
                float mag = conv_radius * ((upper_limit && (radius > max_radius)) ? 1 : (radius / max_radius));
                float rad = Mathf.Atan2(vec.y, vec.x);
                vec.x = mag * AbsCompFloor(Mathf.Cos(rad));
                vec.y = mag * AbsCompFloor(Mathf.Sin(rad));
            }
            return vec;
        }
        /// <summary>
        /// 二点から最大要素を使用して円に補正する
        /// </summary>
        public static Vector2 SetCurcler(Vector2 lhs, Vector2 rhs, float max_radius = 1f)
        {
            return ConvMag(AbsMax2(lhs, rhs), max_radius);
        }
        /// <summary>
        /// 0～360の角度に変換する
        /// </summary>
        /// <param name="minus">trueなら-180～180の範囲になる</param>
        public static float AbsDeg(float deg, bool minus = DEGMINUS)
        {
            deg %= 360f;
            if (deg < 0f) deg = 360f + deg;
            if ((deg > 180f) && DEGMINUS) deg = 360f - deg;
            return deg;
        }
        /// <summary>
        /// 0～360の角度に変換する
        /// </summary>
        /// <param name="minus">trueなら-180～180の範囲になる</param>
        public static Vector3 AbsDeg(Vector3 deg, bool minus = DEGMINUS)
        {
            deg.x = AbsDeg(deg.x, minus);
            deg.y = AbsDeg(deg.y, minus);
            deg.z = AbsDeg(deg.z, minus);
            return deg;
        }

        /// <summary>
        /// ラジアン値から0～360の角度に変換する
        /// </summary>
        /// <param name="minus">trueなら-180～180の範囲になる</param>
        public static float ConvAbsDeg(float rad, bool minus = DEGMINUS)
        {
            float deg = (rad * Mathf.Rad2Deg);
            return AbsDeg(deg, minus);
        }
        /// <summary>
        /// ベクトルから0～360の角度に変換する
        /// </summary>
        /// <param name="minus">trueなら-180～180の範囲になる</param>
        public static float ToAbsDeg(Vector2 vec, bool minus = DEGMINUS, bool clockwise = CLOCKWISE)
        {
            return ConvAbsDeg(Atan2c(vec.y, vec.x, clockwise), DEGMINUS);
        }
        /// <summary>
        /// オイラー角を正規化して、Lerpで最短変化となる角度を求める
        /// </summary>
        public static float EulerShortest(float fromEuler, float toEuler)
        {
            fromEuler = AbsDeg(fromEuler);
            toEuler = AbsDeg(toEuler);
            if (fromEuler < 180f)
            {
                if ((fromEuler + 180f) <= toEuler) toEuler -= 360f;
            }
            else if (fromEuler > 180f)
            {
                if ((fromEuler - 180f) >= toEuler) toEuler += 360f;
            }
            if (fromEuler == 0f) if (toEuler == 360f) toEuler = 0;
            return toEuler;
        }
        /// <summary>
        /// オイラー角を正規化して、Lerpで最短変化となる角度を3つ求める
        /// </summary>
        public static Vector3 EulerShortest(Vector3 fromEuler, Vector3 toEuler, bool xlimit = false)
        {
            if (xlimit) if (Mathf.Abs(toEuler.x) > 90f) toEuler.x = Mathf.Sign(toEuler.x) * 90f;
            toEuler.x = EulerShortest(fromEuler.x, toEuler.x);
            toEuler.y = EulerShortest(fromEuler.y, toEuler.y);
            toEuler.z = EulerShortest(fromEuler.z, toEuler.z);
            return toEuler;
        }
        /// <summary>
        /// 法線ベクトルを求める
        /// </summary>
        public static Vector3 GetTangent(Vector3 vector)
        {
            float dot = Vector3.Dot(Vector3.up, vector);
            if (Mathf.Abs(dot) >= 1f)
            {
                return Mathf.Sign(dot) * Vector3.right;
            } else
            {
                return Vector3.Cross(Vector3.up, vector);
            }
        }
        /// <summary>
        /// 回転から短軸の回転を抽出する、X軸の回転不安要素を取り除き隊
        /// </summary>
        /// <param name="fieldRotation">基準とする回転行列</param>
        /// <param name="fieldAxis">抽出したい軸、Vector3.UpでXZ平面の回転になります</param>
        public static Quaternion ExtractionAxis(Quaternion fieldRotation, Vector3 fieldAxis)
        {
            Vector3 euler = fieldRotation.eulerAngles;
            if (fieldAxis.x > 0)
            {
                if (Mathf.Abs(Mathf.Abs(euler.z) - 180) < 1)
                {
                    euler.y -= 180; euler.z -= 180;
                    euler.x = 180 - euler.x;
                }
            }
            fieldRotation = 
                Quaternion.AngleAxis(fieldAxis.x * euler.x, Vector3.right)
                * Quaternion.AngleAxis(fieldAxis.z * euler.z, Vector3.forward)
                * Quaternion.AngleAxis(fieldAxis.y * euler.y, Vector3.up);
            return fieldRotation;
        }
        /// <summary>
        /// 入力軸から移動ベクトルを決定する
        /// </summary>
        /// <param name="rotVector">入力軸</param>
        /// <param name="axis">回転する軸</param>
        /// <returns>Quaternionとして返す、そのまま掛けて使う</returns>
        public static Quaternion VectorToQuaternion(Vector2 rotVector, Vector3 axis)
        {
            float tan = Mathf.Rad2Deg * Mathf.Atan2(rotVector.x, rotVector.y);
            return Quaternion.AngleAxis(tan, axis);
        }
        /// <summary>
        /// カメラに合わせて移動ベクトルを決定する
        /// </summary>
        /// <param name="rotVector">入力軸</param>
        /// <param name="axis">回転する軸</param> 
        /// <param name="fieldRotation">基準とする回転行列</param>
        /// <param name="fieldAxis">基準とする回転の有効な軸</param>
        /// <returns>そのまま回転に適用することもできます</returns>
        public static Quaternion VectorToQuaternionFix(Vector2 rotVector, Vector3 axis, 
            Quaternion fieldRotation, Vector3 fieldAxis)
        {
            return VectorToQuaternion(rotVector, axis) * ExtractionAxis(fieldRotation, fieldAxis);
        }
        /// <summary>
        /// クォータニオンから軸をとり、-180～180の範囲で角度を求めます
        /// </summary>
        /// <param name="checkQuat">調べたい回転</param>
        /// <param name="axis">調べたい軸</param>
        public static float DegFromQuaternion(Quaternion checkQuat, Vector3 axis)
        {
            Quaternion mainQuat = ExtractionAxis(checkQuat, axis);
            // 度そのもの
            float mainDeg = Quaternion.Angle(mainQuat, Quaternion.identity);
            // 符号が判別できる
            float dot = Quaternion.Dot(mainQuat, Quaternion.AngleAxis(90, axis));
            mainDeg *= Mathf.Sign(dot);
            return mainDeg;
        }
        /// <summary>
        /// 回転に制限を設ける、-180～180の範囲で指定可能
        /// </summary>
        /// <param name="mainQuat">調べたい回転</param>
        /// <param name="axis">調べたい軸</param>
        /// <param name="minDeg">最小</param>
        /// <param name="maxDeg">最大（もし最小＞最大だった場合は最大は最小になる）</param>
        /// <returns>調整後の回転が返される</returns>
        public static Quaternion RotLimit(Quaternion checkQuat, Vector3 axis, float minDeg = -90, float maxDeg = 90)
        {
            if (minDeg > maxDeg) maxDeg = minDeg;
            float rotAngle = 0;
            float mainDeg = DegFromQuaternion(checkQuat, axis);
            //Debug.Log(mainDeg);
            if (mainDeg < minDeg)
                rotAngle = minDeg - mainDeg;
            else if (mainDeg > maxDeg)
                rotAngle = maxDeg - mainDeg;
            if (rotAngle != 0) {
                checkQuat = checkQuat * Quaternion.AngleAxis(rotAngle, axis);
            }
            return checkQuat;
        }
        /// <summary>
        /// 上下90度をキープする
        /// </summary>
        /// <param name="mainQuat">キープさせたいオブジェクト</param>
        /// <returns>条件を満たさなかった場合は引数の値そのものを返す</returns>
        public static Quaternion RotLimitXZ(Quaternion mainQuat)
        {
            if (Mathf.Abs(mainQuat.x) > Mathf.Abs(mainQuat.w) || (Mathf.Abs(mainQuat.y) < Mathf.Abs(mainQuat.z)))
            {
                // x * w で0よりも大きいか、 y * z で0よりも小さければ奥方向へ転がるタイプの回転
                float v_xw, v_yz;
                v_xw = (Mathf.Abs(mainQuat.x) + Mathf.Abs(mainQuat.w)) / 2;
                v_yz = (Mathf.Abs(mainQuat.y) + Mathf.Abs(mainQuat.z)) / 2;
                mainQuat = new Quaternion(
                    v_xw * Mathf.Sign(mainQuat.x), v_yz * Mathf.Sign(mainQuat.y),
                    v_yz * Mathf.Sign(mainQuat.z), v_xw * Mathf.Sign(mainQuat.w));
            }
            return mainQuat;
        }
        public static Vector2 LimitForce(Vector3 forceVector, Vector3 currentVelocity, Vector3 maxVelocity, Vector3 biasMax) {
            if (Mathf.Abs(currentVelocity.x) > Mathf.Abs(maxVelocity.x + biasMax.x)) forceVector.x = 0;
            if (Mathf.Abs(currentVelocity.y) > Mathf.Abs(maxVelocity.y + biasMax.y)) forceVector.y = 0;
            if (Mathf.Abs(currentVelocity.z) > Mathf.Abs(maxVelocity.z + biasMax.z)) forceVector.z = 0;
            return forceVector;
        }
    }
}