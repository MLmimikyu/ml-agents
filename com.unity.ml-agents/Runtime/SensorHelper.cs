using Unity.Barracuda;

namespace Unity.MLAgents.Sensors
{
    /// <summary>
    /// Utility methods related to <see cref="ISensor"/> implementations.
    /// </summary>
    public static class SensorHelper
    {
        /// <summary>
        /// Generates the observations for the provided sensor, and returns true if they equal the
        /// expected values. If they are unequal, errorMessage is also set.
        /// This should not generally be used in production code. It is only intended for
        /// simplifying unit tests.
        /// </summary>
        /// <param name="sensor"></param>
        /// <param name="expected"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public static bool CompareObservation(ISensor sensor, float[] expected, out string errorMessage)
        {
            var numExpected = expected.Length;
            const float fill = -1337f;
            var output = new float[numExpected];
            for (var i = 0; i < numExpected; i++)
            {
                output[i] = fill;
            }

            if (numExpected > 0)
            {
                if (fill != output[0])
                {
                    errorMessage = "Error setting output buffer.";
                    return false;
                }
            }

            ObservationWriter writer = new ObservationWriter();
            writer.SetTarget(output, sensor.GetObservationSpec(), 0);

            // Make sure ObservationWriter didn't touch anything
            if (numExpected > 0)
            {
                if (fill != output[0])
                {
                    errorMessage = "ObservationWriter.SetTarget modified a buffer it shouldn't have.";
                    return false;
                }
            }

            sensor.Write(writer);
            for (var i = 0; i < output.Length; i++)
            {
                if (expected[i] != output[i])
                {
                    errorMessage = $"Expected and actual differed in position {i}. Expected: {expected[i]}  Actual: {output[i]} ";
                    return false;
                }
            }

            errorMessage = null;
            return true;
        }

        /// <summary>
        /// Generates the observations for the provided sensor, and returns true if they equal the
        /// expected values. If they are unequal, errorMessage is also set.
        /// This should not generally be used in production code. It is only intended for
        /// simplifying unit tests.
        /// </summary>
        /// <param name="sensor"></param>
        /// <param name="expected"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public static bool CompareObservation(ISensor sensor, float[,,] expected, out string errorMessage)
        {
            //@Barracude4Upgrade: Format doesn't expect the 4th tensor dimension any more.
            var tensorShape = new TensorShape(expected.GetLength(0), expected.GetLength(1), expected.GetLength(2));
            var numExpected = tensorShape.length;  //@TODO: verify correctness
            const float fill = -1337f;
            var output = new float[numExpected];
            for (var i = 0; i < numExpected; i++)
            {
                output[i] = fill;
            }

            if (numExpected > 0)
            {
                if (fill != output[0])
                {
                    errorMessage = "Error setting output buffer.";
                    return false;
                }
            }

            ObservationWriter writer = new ObservationWriter();
            writer.SetTarget(output, sensor.GetObservationSpec(), 0);

            // Make sure ObservationWriter didn't touch anything
            if (numExpected > 0)
            {
                if (fill != output[0])
                {
                    errorMessage = "ObservationWriter.SetTarget modified a buffer it shouldn't have.";
                    return false;
                }
            }

            sensor.Write(writer);
            for (var h = 0; h < tensorShape[-2]; h++) //@TODO: verify correctness
            {
                for (var w = 0; w < tensorShape[-1]; w++) //@TODO: verify correctness
                {
                    for (var c = 0; c < tensorShape[-3]; c++) //@TODO: verify correctness
                    {
                        if (expected[c, h, w] != output[tensorShape.RavelIndex(0, c, h, w)]) //@TODO: verify correctness  -- also, should equality be tested here or test against some epsilon?
                        {
                            errorMessage = $"Expected and actual differed in position [{h}, {w}, {c}]. " +
                                $"Expected: {expected[c, h, w]}  Actual: {output[tensorShape.RavelIndex(0, c, h, w)]} ";  //@TODO: verify correctness
                            return false;
                        }
                    }
                }
            }
            errorMessage = null;
            return true;
        }
    }
}
