function [cameraParams, imagesUsed, estimationErrors] = calibrateCamera(imagePoints, worldUnits, boardSize, squareSize, estimateSkew, estimateTangentialDistortion, numRadialDistortionCoefficients, mrows, ncols)

% Generate world coordinates of the corners of the squares
worldPoints = generateCheckerboardPoints(boardSize, squareSize);

[cameraParams, imagesUsed, estimationErrors] = estimateCameraParameters(imagePoints, worldPoints, ...
    'EstimateSkew', estimateSkew, ...
    'EstimateTangentialDistortion', estimateTangentialDistortion, ...
    'NumRadialDistortionCoefficients', numRadialDistortionCoefficients, ...
    'WorldUnits', worldUnits, ...
    'InitialIntrinsicMatrix', [], ...
    'InitialRadialDistortion', [], ...
    'ImageSize', [mrows, ncols]); %% Should be the same for all images from the camera?

end