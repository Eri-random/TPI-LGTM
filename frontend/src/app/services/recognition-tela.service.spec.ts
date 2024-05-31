import { TestBed } from '@angular/core/testing';

import { RecognitionTelaService } from './recognition-tela.service';

describe('RecognitionTelaService', () => {
  let service: RecognitionTelaService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(RecognitionTelaService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
